using System;
using Puerts;
#pragma warning disable IL2026
#pragma warning disable IL2111

namespace 创世记;

public static class TsTransform {
	static private Func<string, object?, string, object> _compiler = null!;

	public static JSObject Compile(string code, string fileName) {
		JSObject? res = null;
		Utils.Tree.Root.SyncSend(_ => {
			res = (JSObject)_compiler.Invoke(code, null, fileName);
		});
		return res!;
	}

	public static void Prepare() {
		var env = new JsEnv(new WorldModuleLoader(default));
		env.Eval("const exports = {};");
		env.Eval("console.log = CS.创世记.Log.Debug");
		_compiler = env.ExecuteModule("创世记:typescript").Get<Func<string, object?, string, object>>("transform");
	}
}