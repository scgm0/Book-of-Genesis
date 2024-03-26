using System.Collections.Generic;
using Godot;

namespace 创世记;

public static partial class Utils {
	public static readonly Dictionary<string, string> Polyfill = new() {
		{ "typescript", FileAccess.GetFileAsString("res://Polyfill/typescript.js") },
		{ "events", FileAccess.GetFileAsString("res://Polyfill/events.js") }
	};
}