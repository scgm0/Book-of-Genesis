using System;
using System.Text;
using System.Threading;
using Godot;
using Jint;
using Jint.Native;
using Jint.Runtime;

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
		var index = str.IndexOf(oldValue, StringComparison.Ordinal);
		return index > -1 ? str.Remove(index, oldValue.Length).Insert(index, newValue) : str;
	}

	public static Variant JsValueToVariant(this JsValue jsValue) {
		switch (jsValue.Type) {
			case Types.Boolean: return jsValue.AsBoolean();
			case Types.String: return jsValue.AsString();
			case Types.BigInt: return (int)jsValue.AsNumber();
			case Types.Number: return jsValue.AsNumber();
			case Types.Object: return Json.ParseString(JsonSerializer!.Serialize(jsValue).AsString());
			case Types.Symbol:
			case Types.Empty:
			case Types.Undefined:
			case Types.Null:
			default: return default;
		}
	}

	public static JsValue VariantToJsValue(this Variant variant) {
		switch (variant.VariantType) {
			case Variant.Type.Bool: return variant.AsBool();
			case Variant.Type.Int:
			case Variant.Type.Float: return variant.AsDouble();
			case Variant.Type.String:
			case Variant.Type.StringName: return variant.AsString();
			case Variant.Type.Dictionary:
			case Variant.Type.Array: return JsonParser!.Parse(Json.Stringify(variant));
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