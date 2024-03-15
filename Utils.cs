using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Godot;
using Jint.Native.Json;
using Jint.Runtime;
using World;
using Timer = Godot.Timer;

// ReSharper disable MemberCanBePrivate.Global

namespace 创世记;

public static partial class Utils {
	public const string EncryptionWorldExtension = "worldpack";

	public const string ResWorldsPath = "res://Worlds";
	public const string ResTemplatesPath = "res://Templates";
	public const string TemplateZipPath = "res://Templates.zip";

	public static readonly string ScriptAes256EncryptionKey = SCRIPT_AES256_ENCRYPTION_KEY();

	public static readonly StringName GameVersion = ProjectSettings.GetSetting("application/config/version").AsStringName();

	public static readonly StringName GameName = ProjectSettings.GetSetting("application/config/name").AsStringName();

#if GODOT_ANDROID
	public static readonly string GameUserDataPath = $"{OS.GetSystemDir(OS.SystemDir.Desktop)}/{GameName}";

	public static readonly string SavesPath = $"{GameUserDataPath}/Saves";

	public static readonly string UserWorldsPath = $"{GameUserDataPath}/Worlds";

	public static readonly string TsGenPath = $"{GameUserDataPath}/TsGen";
#else
	public static readonly string GameUserDataPath = ProjectSettings.GlobalizePath("user://");

	public const string SavesPath = "user://Saves";

	public const string UserWorldsPath = "user://Worlds";

	public const string TsGenPath = "user://TsGen";
#endif

	public static readonly string LogPath = ProjectSettings.GetSettingWithOverride("debug/file_logging/log_path").ToString();
	
	public static JsonParser? JsonParser;

	static private System.Threading.Timer? _debounceTimer;

	public static void Debounce(Action action, int delay) {
		_debounceTimer?.Dispose();
		_debounceTimer = new System.Threading.Timer(_ => {
			action.Invoke();
		}, null, delay, Timeout.Infinite);
	}

	public static void ExportEncryptionWorldPck(WorldInfo worldInfo) {
		Log.Debug("加密开始:", worldInfo.JsonString);
		var packer = new PckPacker();
		packer.PckStart($"{UserWorldsPath}/{worldInfo.Name}-{worldInfo.Version}.{EncryptionWorldExtension}",
			32,
			ScriptAes256EncryptionKey,
			true);
		packer.AddDir($"{UserWorldsPath}{worldInfo.Path}");
		packer.AddFile(
			$"{ResWorldsPath}{worldInfo.Path}/{$"{worldInfo.Author}:{worldInfo.Name}-{worldInfo.Version}".EnBase64()}.isEncrypt",
			"res://Assets/.Encrypt");
		packer.Flush(true);
		Log.Debug("加密结束:",
			ProjectSettings.GlobalizePath($"{UserWorldsPath}/{worldInfo.Name}-{worldInfo.Version}.{EncryptionWorldExtension}"));
	}

	public static string ParseExpressionsFilter(string bbcode) {
		var startIndex = bbcode.IndexOf('[') + 1;
		var endIndex = bbcode.IndexOf(']');
		var tagContent = bbcode.Substring(startIndex, endIndex - startIndex);
		var tagParts = tagContent.Split(" ", false).ToList();
		tagParts.RemoveAt(0);

		var filterKey = tagParts.FirstOrDefault(p => p.StartsWith("filter"));
		if (string.IsNullOrEmpty(filterKey)) return string.Empty;

		var filterValue = tagParts.FirstOrDefault(p => p.StartsWith(filterKey));
		return filterValue != null ? filterValue.Split("=", false)[1] : string.Empty;
	}

	public static void CopyDir(string sourceDir, string destDir) {
		if (!DirAccess.DirExistsAbsolute(destDir)) {
			DirAccess.MakeDirRecursiveAbsolute(destDir);
		}

		using var dir = DirAccess.Open(sourceDir);
		if (dir == null) return;
		dir.ListDirBegin();
		var fileName = dir.GetNext();
		while (fileName is not "" and not "." and not "..") {
			if (!dir.CurrentIsDir()) {
				dir.Copy($"{sourceDir}/{fileName}".SimplifyPath(), $"{destDir}/{fileName}".SimplifyPath());
			} else {
				CopyDir($"{sourceDir}/{fileName}".SimplifyPath(), $"{destDir}/{fileName}".SimplifyPath());
			}

			fileName = dir.GetNext();
		}
	}

	public static void RemoveDir(string path) {
		using var dir = DirAccess.Open(path);
		if (dir == null) return;
		dir.ListDirBegin();
		var fileName = dir.GetNext();
		while (fileName is not "" and not "." and not "..") {
			if (!dir.CurrentIsDir()) {
				dir.Remove($"{path}/{fileName}".SimplifyPath());
			} else {
				RemoveDir($"{path}/{fileName}".SimplifyPath());
			}

			fileName = dir.GetNext();
		}

		dir.Remove("./");
	}

	public static int VersionCompare(string version1, string version2) {
		var v1 = new Version(version1);
		var v2 = new Version(version2);
		if (v1 > v2) return 1;
		if (v1 == v2) return 0;
		return -1;
	}

	public static void SetRichText(RichTextLabel label, string text) {
		LoadRichTextImg(ref text);
		label.ParseBbcode(text);
	}

	public static void AddRichText(RichTextLabel label, string text) {
		LoadRichTextImg(ref text);
		label.AppendText(text);
	}

	public static void LoadRichTextImg(ref string text) {
		if (string.IsNullOrEmpty(text)) return;
		foreach (Match match in ImgPathRegex().Matches(text)) {
			var path = match.Groups["path"].Value;
			var oldText = text.Substring(match.Index, match.Length);
			var filter = ParseExpressionsFilter(oldText);

			var texture = filter switch {
				"linear" => LoadImageFile(path),
				"nearest" => LoadImageFile(path, FilterType.Nearest),
				_ => LoadImageFile(path)
			};

			if (texture != null) {
				text = text.Replace(oldText, oldText.Replace(path, texture.ResourcePath));
			}
		}
	}

	public static CanvasTexture? LoadImageFile(string path, FilterType filter = FilterType.Linear) {
		return LoadImageFile(Main.CurrentWorldInfo!, path, (CanvasItem.TextureFilterEnum)filter);
	}

	public static CanvasTexture? LoadImageFile(
		WorldInfo worldInfo,
		string path,
		CanvasItem.TextureFilterEnum filter = CanvasItem.TextureFilterEnum.ParentNode) {
		var filePath = worldInfo.GlobalPath.PathJoin(path).SimplifyPath();
		if (!FileAccess.FileExists(filePath)) return null;
		ImageTexture? imageTexture = null;
		if (ResourceLoader.Exists(filePath) && GD.Load(filePath) is CanvasTexture canvasTexture) {
			if (canvasTexture.TextureFilter == filter) {
				return canvasTexture;
			}

			imageTexture = canvasTexture.DiffuseTexture as ImageTexture;
		}

		var texture = new CanvasTexture();
		texture.TextureFilter = filter;
		texture.TakeOverPath(filePath);
		TextureCache.Add(texture);
		if (imageTexture == null) {
			var data = FileAccess.GetFileAsBytes(filePath);
			using var img = ImageFromBuffer(data);
			imageTexture = ImageTexture.CreateFromImage(img);
		}

		texture.DiffuseTexture = imageTexture;

		return texture;
	}

	public static Image ImageFromBuffer(byte[] data) {
		var img = new Image();
		switch (ImageFileFormatFinder.GetImageFormat(data)) {
			case ImageFormat.Png:
				img.LoadPngFromBuffer(data);
				break;
			case ImageFormat.Jpg:
				img.LoadJpgFromBuffer(data);
				break;
			case ImageFormat.Bmp:
				img.LoadBmpFromBuffer(data);
				break;
			case ImageFormat.Webp:
				img.LoadWebpFromBuffer(data);
				break;
			case ImageFormat.Unknown:
			default:
				throw new JavaScriptException("不支持的图像格式，仅支持png、jpg、bmp与webp");
		}

		return img;
	}

	static private partial string SCRIPT_AES256_ENCRYPTION_KEY();

	[GeneratedRegex(@"(?<=(//# sourceMappingURL=))[.\s\S]*?", RegexOptions.RightToLeft)]
	public static partial Regex SourceMapPathRegex();

	[GeneratedRegex(@"\[img.*\](?<path>.*?)\[\/img\]")]
	public static partial Regex ImgPathRegex();

	[GeneratedRegex(@"^\w:[\\\/]")]
	public static partial Regex DriveLetterRegex();

}