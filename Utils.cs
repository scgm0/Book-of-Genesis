using System.Linq;
using System.Text.RegularExpressions;
using Godot;

// ReSharper disable MemberCanBePrivate.Global

namespace 创世记;

public static partial class Utils {
	public const string EncryptionWorldExtension = "worldpack";

	public const string ResWorldsPath = "res://Worlds";
	public const string ResTemplatesPath = "res://Templates";
	public const string TemplateZipPath = "res://Templates.zip";

	public static readonly string ScriptAes256EncryptionKey = SCRIPT_AES256_ENCRYPTION_KEY();

	public static readonly bool IsAndroid = OS.GetName() == "Android";

	public static readonly StringName GameVersion = ProjectSettings.GetSetting("application/config/version").AsStringName();

	public static readonly StringName AppName = ProjectSettings.GetSetting("application/config/name").AsStringName();

	public static readonly string SavesPath =
		IsAndroid ? $"{OS.GetSystemDir(OS.SystemDir.Desktop)}/{AppName}/Saves" : "user://Saves";

	public static readonly string UserWorldsPath =
		IsAndroid ? $"{OS.GetSystemDir(OS.SystemDir.Desktop)}/{AppName}/Worlds" : "user://Worlds";

	public static readonly string TsGenPath =
		IsAndroid ? $"{OS.GetSystemDir(OS.SystemDir.Desktop)}/{AppName}/TsGen" : "user://TsGen";

	public static void ExportEncryptionWorldPck(WorldInfo worldInfo) {
		Main.Log("加密开始:", worldInfo.JsonString);
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
		Main.Log("加密结束:", ProjectSettings.GlobalizePath($"{UserWorldsPath}/{worldInfo.Name}-{worldInfo.Version}.{EncryptionWorldExtension}"));
	}

	public static string ParseExpressionsForValues(string bbcode) {
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

	static private partial string SCRIPT_AES256_ENCRYPTION_KEY();

	[GeneratedRegex(@"(?<=(//# sourceMappingURL=))[.\s\S]*?", RegexOptions.RightToLeft)]
	public static partial Regex SourceMapPathRegex();

	[GeneratedRegex(@"\[img.*\](?<path>.*?)\[\/img\]")]
	public static partial Regex ImgPathRegex();

}