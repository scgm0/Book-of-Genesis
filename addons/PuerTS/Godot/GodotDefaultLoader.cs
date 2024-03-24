using System;
using Puerts;
using FileAccess = Godot.FileAccess;

// ReSharper disable once CheckNamespace
public class GodotDefaultLoader : ILoader {
	public bool FileExists(string filepath) {
		return FileAccess.FileExists("res://addons/PuerTS/Runtime/Resources/" + filepath) ||
			FileAccess.FileExists("res://addons/PuerTS/Editor/Resources/" + filepath);
	}

	public string ReadFile(string filepath, out string debugpath) {
		if (FileAccess.FileExists("res://addons/PuerTS/Runtime/Resources/" + filepath)) {
			debugpath = "res://addons/PuerTS/Runtime/Resources/" + filepath;
		} else if (FileAccess.FileExists("res://addons/PuerTS/Editor/Resources/" + filepath)) {
			debugpath = "res://addons/PuerTS/Editor/Resources/" + filepath;
		} else {
			throw new Exception("file not found: " + filepath);
		}

		return FileAccess.GetFileAsString(debugpath);
	}
}