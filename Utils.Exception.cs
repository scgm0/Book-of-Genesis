using System;
using System.Numerics;
using System.Text;
using System.Threading;
using Godot;
using Godot.Collections;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime;

namespace 创世记;

public static partial class Utils {

	public static void SyncSend(this Node node, SendOrPostCallback action) => SyncCtx.Send(action, node);

	public static void SyncPost(this Node node, SendOrPostCallback action) => SyncCtx.Post(action, node);

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

	enum Js2DicType {
		BigInt,
		Date
	}

	public static Variant JsValueToVariant(this JsValue jsValue) {
		switch (jsValue.Type) {
			case Types.Boolean: return jsValue.AsBoolean();
			case Types.String: return jsValue.AsString();
			case Types.BigInt: {
				var dic = new Dictionary();
				dic["Js2DicType"] = (int)Js2DicType.BigInt;
				dic["value"] = TypeConverter.ToBigInt(jsValue).ToByteArray();
				return dic;
			}
			case Types.Number: return jsValue.AsNumber();
			case Types.Object: {
				if (jsValue is Function) return default;

				if (jsValue.IsFloat32Array()) {
					return jsValue.AsFloat32Array();
				}

				if (jsValue.IsFloat64Array()) {
					return jsValue.AsFloat64Array();
				}

				if (jsValue.IsInt32Array() || jsValue.IsUint32Array() || jsValue.IsInt16Array() || jsValue.IsUint16Array() || jsValue.IsInt8Array()) {
					return jsValue.AsInt32Array();
				}

				if (jsValue.IsUint8Array() || jsValue.IsUint8ClampedArray()) {
					return jsValue.AsUint8Array();
				}

				if (jsValue.IsBigInt64Array() || jsValue.IsBigUint64Array()) {
					return jsValue.AsBigInt64Array();
				}

				var dic = new Dictionary();

				if (jsValue.IsDate()) {
					dic["Js2DicType"] = (int)Js2DicType.Date;
					dic["value"] = jsValue.AsDate().DateValue;
					return dic;

				}

				var obj = jsValue.AsObject();
				foreach (var (key, propertyDescriptor) in obj.GetOwnProperties()) {
					dic[key.JsValueToVariant()] = propertyDescriptor.Value.JsValueToVariant();
				}

				return dic;
			}
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
			case Variant.Type.Dictionary: {
				var dic = variant.AsGodotDictionary();
				if (dic.TryGetValue("Js2DicType", out var type) && type.VariantType == Variant.Type.Int) {
					switch ((int)type) {
						case (int)Js2DicType.BigInt: {
							var bytes = dic["value"].AsByteArray();
							return new JsBigInt(new BigInteger(bytes));
						}
						case (int)Js2DicType.Date: {
							var date = dic["value"].AsInt64();
							return new JsDate(Main.CurrentEngine!, date);
						}
					}
				}

				var obj = new JsObject(Main.CurrentEngine!);
				foreach (var (key, value) in dic) {
					obj[key.VariantToJsValue()] = value.VariantToJsValue();
				}

				return obj;
			}
			case Variant.Type.Array: {
				var jsArray = new JsArray(Main.CurrentEngine!);
				var array = variant.AsGodotArray();
				foreach (var item in array) {
					jsArray.Push(item.VariantToJsValue());
				}

				return jsArray;
			}
			case Variant.Type.PackedByteArray: {
				var bytes = variant.AsByteArray();
				return Main.CurrentEngine!.Intrinsics.Uint8Array.Construct(bytes);
			}
			case Variant.Type.PackedInt32Array: {
				var bytes = variant.AsInt32Array();
				return Main.CurrentEngine!.Intrinsics.Int32Array.Construct(bytes);
			}
			case Variant.Type.PackedInt64Array: {
				var bytes = variant.AsInt64Array();
				return Main.CurrentEngine!.Intrinsics.BigInt64Array.Construct(bytes);

			}
			case Variant.Type.PackedFloat32Array: {
				var bytes = variant.AsFloat32Array();
				return Main.CurrentEngine!.Intrinsics.Float32Array.Construct(bytes);

			}
			case Variant.Type.PackedFloat64Array: {
				var bytes = variant.AsFloat64Array();
				return Main.CurrentEngine!.Intrinsics.Float64Array.Construct(bytes);

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