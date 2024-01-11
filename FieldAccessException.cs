using Godot;

// ReSharper disable once CheckNamespace
public static class FieldAccessException {
	public static uint Peek(this FileAccess file) {
		var pos = file.GetPosition();
		var @char = file.Get8();
		file.Seek(pos);
		return @char;
	}

	public static void Skip(this FileAccess file, ulong length) {
		file.Seek(file.GetPosition() + length);
	}
}