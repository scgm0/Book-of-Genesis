using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Godot;
using Godot.Collections;
using Jint;
using Jint.Native;
using Jint.Native.Json;
using SourceMaps;
using Engine = Jint.Engine;
using JsonSerializer = Jint.Native.Json.JsonSerializer;

namespace 创世记;

public static partial class Utils {
	static private readonly string ScriptAes256EncryptionKey = SCRIPT_AES256_ENCRYPTION_KEY();

	public static readonly bool IsAndroid = OS.GetName() == "Android";

	// ReSharper disable once MemberCanBePrivate.Global
	public static readonly string AppName = ProjectSettings.GetSetting("application/config/name").AsStringName();

	public static readonly string SavesPath =
		IsAndroid ? $"{OS.GetSystemDir(OS.SystemDir.Desktop)}/{AppName}/Saves" : "user://Saves";

	public static readonly string UserModsPath =
		IsAndroid ? $"{OS.GetSystemDir(OS.SystemDir.Desktop)}/{AppName}/Mods" : "user://Mods";

	public static readonly string TsGenPath =
		IsAndroid ? $"{OS.GetSystemDir(OS.SystemDir.Desktop)}/{AppName}/TsGen" : "user://TsGen";

	public const string ResModsPath = "res://Mods";

	public static SourceMapCollection SourceMapCollection { get; set; } = new();
	public static readonly TsTransform TsTransform = new();
	public static readonly SceneTree Tree = Godot.Engine.GetMainLoop() as SceneTree;
	public static readonly List<Texture2D> TextureCache = [];

	static Utils() {
		if (!DirAccess.DirExistsAbsolute(UserModsPath)) {
			DirAccess.MakeDirRecursiveAbsolute(UserModsPath);
		}

		if (!DirAccess.DirExistsAbsolute(SavesPath)) {
			DirAccess.MakeDirRecursiveAbsolute(SavesPath);
		}

		if (!DirAccess.DirExistsAbsolute(TsGenPath)) {
			DirAccess.MakeDirRecursiveAbsolute(TsGenPath);
		}
	}

	public static class Polyfill {
		public static readonly string Events = FileAccess.GetFileAsString("res:///Polyfill/Events.js");
		public static readonly string Tsc = FileAccess.GetFileAsString("res:///Polyfill/Tsc.js");
	}

	public static void ExportEncryptionModPck(ModInfo modInfo) {
		var packer = new PckPacker();
		packer.PckStart($"{UserModsPath}/{modInfo.Name}-{modInfo.Version}.modpack",
			32,
			ScriptAes256EncryptionKey,
			true);
		PckAddDir(packer, $"{UserModsPath}{modInfo.Path}");
		packer.AddFile(
			$"{ResModsPath}{modInfo.Path}/{$"{modInfo.Author}:{modInfo.Name}-{modInfo.Version}".EnBase64()}.isEncrypt",
			"res://Assets/.Encrypt");
		packer.Flush(true);
	}

	static private void PckAddDir(PckPacker packer, string path) {
		using var dir = DirAccess.Open(path);
		if (dir == null) return;
		dir.ListDirBegin();
		var fileName = dir.GetNext();
		while (fileName is not "" and not "." and not "..") {
			var filePath = path.PathJoin(fileName);
			if (dir.CurrentIsDir()) {
				PckAddDir(packer, filePath);
			} else {
				packer.AddFile(filePath.ReplaceOnce(UserModsPath, ResModsPath), filePath, true);
			}

			fileName = dir.GetNext();
		}
	}

	//替换字符串中的某个值
	public static string ReplaceOnce(this string str, string oldValue, string newValue) {
		//获取字符串中oldValue的索引
		var index = str.IndexOf(oldValue, StringComparison.Ordinal);
		//如果存在oldValue，则替换一次，否则返回原字符串
		return index > -1 ? str.Remove(index, oldValue.Length).Insert(index, newValue) : str;
	}

	public static Variant JsValueToVariant(this JsValue jsValue, Engine engine) {
		//如果jsValue是布尔值，则返回布尔值
		if (jsValue.IsBoolean()) return jsValue.AsBoolean();
		//如果jsValue是数字，则返回数字
		if (jsValue.IsNumber()) return jsValue.AsNumber();
		//如果jsValue是字符串，则返回字符串
		if (jsValue.IsString()) return jsValue.AsString();
		//如果jsValue是对象，则使用JsonSerializer将对象序列化，并返回字符串
		return jsValue.IsObject() ? new JsonSerializer(engine).Serialize(jsValue).ToString() : null;
	}

	public static JsValue VariantToJsValue(this Variant variant, Engine engine) {
		// 根据Variant的类型，返回不同的JsValue
		switch (variant.VariantType) {
			case Variant.Type.Bool:
				// 返回布尔值
				return variant.AsBool();
			case Variant.Type.Int:
			case Variant.Type.Float:
				// 返回浮点数
				return variant.AsDouble();
			case Variant.Type.String:
				// 获取字符串
				var str = variant.ToString();
				// 释放Variant
				variant.Dispose();
				try {
					// 尝试解析字符串
					return new JsonParser(engine).Parse(str);
				} catch (Exception) {
					// 如果解析失败，返回字符串
					return str;
				}
			case Variant.Type.Object:
			case Variant.Type.Nil:
			case Variant.Type.Vector2:
			case Variant.Type.Vector2I:
			case Variant.Type.Rect2:
			case Variant.Type.Rect2I:
			case Variant.Type.Vector3:
			case Variant.Type.Vector3I:
			case Variant.Type.Transform2D:
			case Variant.Type.Vector4:
			case Variant.Type.Vector4I:
			case Variant.Type.Plane:
			case Variant.Type.Quaternion:
			case Variant.Type.Aabb:
			case Variant.Type.Basis:
			case Variant.Type.Transform3D:
			case Variant.Type.Projection:
			case Variant.Type.Color:
			case Variant.Type.StringName:
			case Variant.Type.NodePath:
			case Variant.Type.Rid:
			case Variant.Type.Callable:
			case Variant.Type.Signal:
			case Variant.Type.Dictionary:
			case Variant.Type.Array:
			case Variant.Type.PackedByteArray:
			case Variant.Type.PackedInt32Array:
			case Variant.Type.PackedInt64Array:
			case Variant.Type.PackedFloat32Array:
			case Variant.Type.PackedFloat64Array:
			case Variant.Type.PackedStringArray:
			case Variant.Type.PackedVector2Array:
			case Variant.Type.PackedVector3Array:
			case Variant.Type.PackedColorArray:
			case Variant.Type.Max:
			default:
				// 释放Godot对象
				variant.AsGodotObject().Free();
				// 返回未定义
				return JsValue.Undefined;
		}
	}

	public static string EnBase64(this string str) {
		return string.IsNullOrEmpty(str) ? "" : Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
	}

	public static string UnEnBase64(this string str) {
		return string.IsNullOrEmpty(str) ? "" : Encoding.UTF8.GetString(Convert.FromBase64String(str));
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


	static private partial string SCRIPT_AES256_ENCRYPTION_KEY();

	[GeneratedRegex(@"(?<=(//# sourceMappingURL=))[.\s\S]*?", RegexOptions.RightToLeft)]
	public static partial Regex SourceMapPathRegex();

	[GeneratedRegex(@"\[img.*\](?<path>.*?)\[\/img\]")]
	public static partial Regex ImgPathRegex();
}