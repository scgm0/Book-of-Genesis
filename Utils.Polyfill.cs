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
		{ "source-map-support", FileAccess.GetFileAsString("res://Polyfill/source-map-support.js") }
	};

	public static class InputActionName {
		public static readonly StringName UiAccept = "ui_accept";
	}

	public static class ThemeStyleBoxName {
		public static readonly StringName Normal = "normal";
		public static readonly StringName Focus = "focus";
		public static readonly StringName ReadOnly = "read_only";
		public static readonly StringName Panel = "panel";
	}

	public static class ThemeColorName {
		public static readonly StringName DefaultColor = "default_color";
		public static readonly StringName FontReadonlyColor = "font_readonly_color";
	}
	
	public static class ThemeConstantName {
		public static readonly StringName MarginTop = "margin_top";
		public static readonly StringName MarginBottom = "margin_bottom";
		public static readonly StringName MarginLeft = "margin_left";
		public static readonly StringName MarginRight = "margin_right";
	}
}