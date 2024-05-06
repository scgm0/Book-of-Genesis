using System;
using System.Text;
using System.Threading;
using Godot;

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

	public static string EnBase64(this string str) {
		return string.IsNullOrEmpty(str) ? "" : Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
	}

	public static string UnEnBase64(this string str) {
		return string.IsNullOrEmpty(str) ? "" : Encoding.UTF8.GetString(Convert.FromBase64String(str));
	}

	static private void AddDir(this PckPacker packer, string path, string oldValue, string newValue) {
		using var dir = DirAccess.Open(path);
		if (dir == null) return;
		dir.ListDirBegin();
		var fileName = dir.GetNext();
		while (fileName is not "" and not "." and not "..") {
			var filePath = path.PathJoin(fileName);
			if (dir.CurrentIsDir()) {
				packer.AddDir(filePath, oldValue, newValue);
			} else {
				packer.AddFile(filePath.Replace(oldValue, newValue), filePath, true);
			}

			fileName = dir.GetNext();
		}
	}
}