#if TOOLS

using Godot;

[Tool]
// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
// ReSharper disable once IdentifierTypo
public partial class PuerTSPlugin : EditorPlugin {

	public override void _EnterTree() {
		AddAutoloadSingleton("PuerTS", "res://addons/PuerTS/Godot/PuerTSMenu.cs");
		AddToolSubmenuItem("PuerTS",
			GD.Load<PackedScene>("res://addons/PuerTS/Godot/PuerTSMenu.tscn")
				.Instantiate<PuerTSMenu>()
		);
	}

	public override void _EnablePlugin() {
		AddToolSubmenuItem("PuerTS",
			GD.Load<PackedScene>("res://addons/PuerTS/Godot/PuerTSMenu.tscn")
				.Instantiate<PuerTSMenu>()
		);
	}

	public override void _DisablePlugin() { RemoveToolMenuItem("PuerTS"); }

	public override void _ExitTree() { RemoveToolMenuItem("PuerTS"); }
}

#endif