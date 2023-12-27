using Godot;
using Jint.Runtime.Interop;
using Engine = Jint.Engine;

namespace 创世记;

public partial class Test : Node2D {
	public override void _Ready() {
		var engine = new Engine();
		// Console.Write();
		engine.SetValue("A", TypeReference.CreateTypeReference(engine, typeof(A)));
		engine.Execute("A.setLog(555);");
	}

	private class A {
		// ReSharper disable once UnusedMember.Local
		public static void SetLog(string str) {
			GD.Print(str);
		}
	}
}