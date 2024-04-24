using System;
using Puerts;

namespace 创世记;

public static class TsTransform {
	public static string? TypeScriptVersion { get; private set; }
	static private Func<string, object?, string, JSObject> _compiler = null!;

	public static JSObject Compile(string code, string fileName) {
		return _compiler(code, null, fileName);
	}

	public static void Prepare() {
		var env = new JsEnv(new WorldModuleLoader());
		env.Eval("console.log = CS.创世记.Log.Debug");
		var ts = env.ExecuteModule("创世记:typescript");
		_compiler = ts.Get<Func<string, object?, string, JSObject>>("transform");
		TypeScriptVersion = ts.Get<string>("version")!;
	}
}