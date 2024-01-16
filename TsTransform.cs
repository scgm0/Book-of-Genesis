using System;
using Jint;
using Jint.Native;
using Engine = Jint.Engine;

namespace 创世记;

public static class TsTransform {
	static private readonly Engine Engine = new(options => options.DebugMode(false));
	static private JsValue _compiler = null!;
	public static JsObject Compile(string code, string fileName) => (JsObject)_compiler.Call(code, JsValue.Undefined, fileName);

	public static void Prepare() {
		Engine.SetValue("log", new Action<string[]>(Main.Log));
		Engine.Execute("const global = this;");
		Engine.Execute("const exports = {};");

		Engine.Execute(Utils.Polyfill.Tsc);

		_compiler = Engine.Evaluate("exports.transform");
	}
}