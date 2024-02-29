using Esprima.Ast;
using Godot;
using Script = Esprima.Ast.Script;

namespace 创世记;

public static partial class Utils {
	public static class Polyfill {
		public static readonly Module Events = Jint.Engine.PrepareModule(FileAccess.GetFileAsString("res:///Polyfill/Events.js"));
		public static readonly Script Tsc = Jint.Engine.PrepareScript(FileAccess.GetFileAsString("res:///Polyfill/Tsc.js"));
	}
}