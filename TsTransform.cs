using Godot;
using Jint;
using Jint.Native;
using Engine = Jint.Engine;

namespace 创世记;

public class TsTransform {
	private readonly Engine _engine = new(options => options.DebugMode(false));
	private JsValue _compiler;
	public JsObject Compile(string code, string fileName) => (JsObject)_engine.Invoke(_compiler, code, null, fileName);

	public void Prepare() {
		_engine.SetValue("log", GD.PrintS);
		_engine.Execute("const global = this;");
		_engine.Execute("const exports = {};");

		_engine.Execute(Utils.Polyfill.Tsc);

		_compiler = _engine.Evaluate("exports.transform");
	}
}