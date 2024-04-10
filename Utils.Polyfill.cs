using System.Collections.Generic;
using Godot;

namespace 创世记;

public static partial class Utils {
	public static readonly Dictionary<string, string> Polyfill = new() {
		{ "typescript", FileAccess.GetFileAsString("res://Polyfill/typescript.js") },
		{ "world-init", FileAccess.GetFileAsString("res://Polyfill/world_init.js") },
		{ "event", FileAccess.GetFileAsString("res://Polyfill/event.js") },
		{ "console", FileAccess.GetFileAsString("res://Polyfill/console.js") },
		{ "audio", FileAccess.GetFileAsString("res://Polyfill/audio.js") },
		{"source-map-support",  FileAccess.GetFileAsString("res://Polyfill/source-map-support.js")}
	};
}