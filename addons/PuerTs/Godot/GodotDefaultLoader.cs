using Puerts;
using FileAccess = Godot.FileAccess;

// ReSharper disable once CheckNamespace
#pragma warning disable CA1050

public class GodotDefaultLoader : ILoader {
	public bool FileExists(string filepath) {
		return FileAccess.FileExists("res://addons/PuerTs/Resources/" + filepath);
	}

	public string ReadFile(string filepath, out string debugpath) {
		debugpath = "res://addons/PuerTs/Resources/" + filepath;
		return FileAccess.GetFileAsString(debugpath);
	}
}