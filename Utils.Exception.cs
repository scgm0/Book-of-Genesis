using System;
using System.Text;
using System.Threading;
using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Json;
using Engine = Jint.Engine;

namespace 创世记;

public static partial class Utils {

	public static void SyncSend(this Node node, SendOrPostCallback action) => Dispatcher.SynchronizationContext.Send(action, node);

	public static void SyncPost(this Node node, SendOrPostCallback action) => Dispatcher.SynchronizationContext.Post(action, node);

	public static uint Peek(this FileAccess file) {
		var pos = file.GetPosition();
		var @char = file.Get8();
		file.Seek(pos);
		return @char;
	}

	public static void Skip(this FileAccess file, ulong length) {
		file.Seek(file.GetPosition() + length);
	}

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
		return (jsValue.IsObject() ? new JsonSerializer(engine).Serialize(jsValue).ToString() : null) ?? string.Empty;
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
			case Variant.Type.StringName:
				// 获取字符串
				var str = variant.ToString();
				// 释放Variant
				variant.Dispose();
				try {
					// 尝试解析字符串
					return JsonParser?.Parse(str) ?? str;
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

	static private void AddDir(this PckPacker packer, string path) {
		using var dir = DirAccess.Open(path);
		if (dir == null) return;
		dir.ListDirBegin();
		var fileName = dir.GetNext();
		while (fileName is not "" and not "." and not "..") {
			var filePath = path.PathJoin(fileName);
			if (dir.CurrentIsDir()) {
				packer.AddDir(filePath);
			} else {
				packer.AddFile(filePath.ReplaceOnce(UserWorldsPath, ResWorldsPath), filePath, true);
			}

			fileName = dir.GetNext();
		}
	}
}