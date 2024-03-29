#if TOOLS

using Godot;
// ReSharper disable CheckNamespace
#pragma warning disable CA1050

[Tool]
public partial class PuerTsPlugin : EditorPlugin {
	public override void _EnterTree() {
		AddToolSubmenuItem("PuerTs",
			GD.Load<PackedScene>("res://addons/PuerTs/Godot/PuerTsMenu.tscn")
				.Instantiate<PuerTsMenu>()
		);
	}

	public override void _EnablePlugin() {
		AddToolSubmenuItem("PuerTs",
			GD.Load<PackedScene>("res://addons/PuerTs/Godot/PuerTsMenu.tscn")
				.Instantiate<PuerTsMenu>()
		);
	}

	public override void _ExitTree() { RemoveToolMenuItem("PuerTs"); }
	public override void _DisablePlugin() { RemoveToolMenuItem("PuerTs"); }
}

#endif