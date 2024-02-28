using System;
using Jint;
using Jint.Native;
using Engine = Jint.Engine;

#pragma warning disable IL2026
#pragma warning disable IL2111

namespace 创世记;

public static class TsTransform {
	static private readonly Engine Engine = new(options => options.DebugMode(false));
	static private JsValue _compiler = null!;
	public static JsObject Compile(string code, string fileName) {
		JsObject? res = null;
		Utils.Tree.Root.SyncSend(_ => {
			res = (JsObject)_compiler.Call(code, JsValue.Undefined, fileName);
		});
		return res!;
	}

	public static void Prepare() {
		Engine.SetValue("log", new Action<string[]>(Main.Log));
		Engine.Execute("const global = this;");
		Engine.Execute("const exports = {};");

		Engine.Execute(Utils.Polyfill.Tsc);

		_compiler = Engine.Evaluate("exports.transform");
	}
}