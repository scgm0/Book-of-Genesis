using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Godot;
using Godot.Collections;
using Puerts;
using Array = Godot.Collections.Array;
using Timer = System.Threading.Timer;

namespace 创世记;

public static partial class Utils {
	public const string EncryptionWorldExtension = "worldpack";

	public const string ResWorldsPath = "res://Worlds";
	public const string ResTemplatesPath = "res://Templates";
	public const string TemplateZipPath = "res://Templates.zip";

	public static readonly string ScriptAes256EncryptionKey = SCRIPT_AES256_ENCRYPTION_KEY();

	public static readonly string GameVersion = ProjectSettings.GetSetting("application/config/version").AsString();

	public static readonly string GameName = ProjectSettings.GetSetting("application/config/name").AsString();

#if GODOT_ANDROID
	public static readonly string GameUserDataPath = $"{OS.GetSystemDir(OS.SystemDir.Desktop)}/{GameName}";
#else
	public static readonly string GameUserDataPath = ProjectSettings.GlobalizePath("user://");
#endif

	public static readonly string SavesPath = $"{GameUserDataPath}/Saves";

	public static readonly string UserWorldsPath = $"{GameUserDataPath}/Worlds";

	public static readonly string TsGenPath = $"{GameUserDataPath}/TsGen";

	public static readonly string LogPath = ProjectSettings.GetSettingWithOverride("debug/file_logging/log_path").ToString();

	public static readonly GodotSynchronizationContext SyncCtx = Dispatcher.SynchronizationContext;

	static private Timer? _debounceTimer;

	public static void Debounce(Action action, int delay) {
		_debounceTimer?.Dispose();
		_debounceTimer = new Timer(_ => {
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
		packer.Flush();
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

	public static void LoadPacks(string path) {
		using var dir = DirAccess.Open(path);
		if (dir == null) return;
		dir.ListDirBegin();
		var fileName = dir.GetNext();
		while (fileName is not "" and not "." and not "..") {
			if (!dir.CurrentIsDir() && (fileName.GetExtension() == EncryptionWorldExtension || fileName.GetExtension() == "zip")) {
				Log.Debug(fileName, ProjectSettings.LoadResourcePack($"{path}/{fileName}".SimplifyPath()).ToString());
			}

			fileName = dir.GetNext();
		}
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
		return Main.CurrentWorldInfo is null ? null : LoadImageFile(Main.CurrentWorldInfo, path, (CanvasItem.TextureFilterEnum)filter);
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
				throw new Exception("不支持的图像格式，仅支持png、jpg、bmp与webp");
		}

		return img;
	}

	public static void RemoveButtonById(ulong id) {
		var button = GodotObject.InstanceFromId(id) as Button;
		button?.QueueFree();
	}

	static private object? VariantToSaveValue(Variant value) {
		return value.VariantType switch {
			Variant.Type.Bool => value.AsBool(),
			Variant.Type.Int => value.AsInt32(),
			Variant.Type.Float => value.AsDouble(),
			Variant.Type.String => value.AsString(),
			Variant.Type.Dictionary => value.AsGodotDictionary(),
			Variant.Type.Array => value.AsGodotArray(),
			Variant.Type.Vector2 or Variant.Type.Vector2I or Variant.Type.Rect2 or Variant.Type.Rect2I or Variant.Type.Vector3 or Variant.Type.Vector3I or Variant.Type.Transform2D or Variant.Type.Vector4
				or Variant.Type.Vector4I or Variant.Type.Plane or Variant.Type.Quaternion or Variant.Type.Aabb or Variant.Type.Basis or Variant.Type.Transform3D or Variant.Type.Projection or Variant.Type.Color
				or Variant.Type.StringName or Variant.Type.NodePath or Variant.Type.Rid or Variant.Type.Object or Variant.Type.Callable or Variant.Type.Signal or Variant.Type.PackedByteArray or Variant.Type.PackedInt32Array
				or Variant.Type.PackedInt64Array or Variant.Type.PackedFloat32Array or Variant.Type.PackedFloat64Array or Variant.Type.PackedStringArray or Variant.Type.PackedVector2Array or Variant.Type.PackedVector3Array
				or Variant.Type.PackedColorArray or Variant.Type.Max or Variant.Type.Nil => null,
			_ => null
		};
	}

	static private Variant SaveValueToVariant(object? value) {
		switch (value) {
			case bool boolValue:
				return boolValue;
			case int intValue:
				return intValue;
			case double doubleValue:
				return doubleValue;
			case string stringValue:
				return stringValue;
			case JSObject jsObject: {
				var str = jsObject.Get<string>("value");
				using var json = new Json();
				json.Parse(str);
				return json.Data;
			}
			default:
				return default;
		}
	}

	public static object? GetSaveValue(string section, string key) {
		using var defaultVariant = new RefCounted();
		using var value = Main.CurrentWorldInfo!.Config.GetValue(section, key, defaultVariant);
		return VariantToSaveValue(value);
	}

	public static void SetSaveValue(string section, string key, object? value) {
		Main.CurrentWorldInfo!.Config.SetValue(section, key, SaveValueToVariant(value));
		Main.CurrentWorldInfo.Config.SaveEncryptedPass(
			$"{SavesPath}/{Main.CurrentWorldInfo.Author}:{Main.CurrentWorldInfo.Name}.save",
			$"{Main.CurrentWorldInfo.Author}:{Main.CurrentWorldInfo.Name}");
	}

	public static object? GetGlobalSaveValue(string section, string key) {
		using var defaultVariant = new RefCounted();
		using var value = GlobalConfig.GetValue(section, key, defaultVariant);
		return VariantToSaveValue(value);
	}

	public static void SetGlobalSaveValue(string section, string key, object? value) {
		GlobalConfig.SetValue(section, key, SaveValueToVariant(value));
		GlobalConfig.SaveEncryptedPass($"{SavesPath}/global.save", "global");
	}

	public static string? ReadAsText(string path) {
		var filePath = Main.CurrentWorldInfo!.GlobalPath.PathJoin(path).SimplifyPath();
		return FileAccess.FileExists(filePath) ? FileAccess.GetFileAsString(filePath) : null;
	}

	public static ArrayBuffer? ReadAsArrayBuffer(string path) {
		var filePath = Main.CurrentWorldInfo!.GlobalPath.PathJoin(path).SimplifyPath();
		return FileAccess.FileExists(filePath) ? new ArrayBuffer(FileAccess.GetFileAsBytes(filePath)) : null;
	}

	public static string GetJsonString(Dictionary dictionary) {
		return Json.Stringify(dictionary);
	}

	public static string GetJsonString(Array array) {
		return Json.Stringify(array);
	}

	static private partial string SCRIPT_AES256_ENCRYPTION_KEY();

	[GeneratedRegex(@"\[img.*\](?<path>.*?)\[\/img\]")]
	public static partial Regex ImgPathRegex();

	[GeneratedRegex(@"^\w:[\\\/]")]
	public static partial Regex DriveLetterRegex();

}