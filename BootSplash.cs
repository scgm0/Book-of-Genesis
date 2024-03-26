using Godot;

namespace 创世记;

public partial class BootSplash : Control {
	[GetNode("%Logo")] private TextureRect _logo;

	[Export(PropertyHint.File, "*.tscn")] private string _scenePath;

	public override void _Ready() {
		Control instancedScene = null;
		_logo.Modulate = _logo.Modulate with { A = 0 };
		var tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Cubic);
		tween.TweenProperty(_logo, "modulate:a", 1, 1);
		tween.TweenCallback(Callable.From(() => {
			using var packedScene = ResourceLoader.Load<PackedScene>(_scenePath);
			instancedScene = packedScene?.Instantiate<Control>();
			if (instancedScene == null) return;
			instancedScene.Visible = false;
			AddSibling(instancedScene);
			Utils.Tree.Root.MoveChild(instancedScene, -2);
			Utils.Tree.CurrentScene = instancedScene;
		}));
		tween.TweenCallback(Callable.From(() => { instancedScene!.Visible = true; })).SetDelay(0.05);
		tween.Parallel().TweenProperty(this, "modulate:a", 0, 0.5);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}