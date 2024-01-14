using Godot;

namespace 创世记;

public static partial class Utils {
	public static class Polyfill {
		public static readonly string Events = FileAccess.GetFileAsString("res:///Polyfill/Events.js");
		public static readonly string Tsc = FileAccess.GetFileAsString("res:///Polyfill/Tsc.js");
	}
}