using Godot;

namespace 创世记;

public partial class BootSplash : Control {
	[GetNode("%Logo")] private TextureRect _logo;

	[Export(PropertyHint.File, "*.tscn")] private string _scenePath;

	public override void _Ready() {
		Control? instancedScene = null;
		_logo.Modulate = _logo.Modulate with { A = 0 };
		ResourceLoader.LoadThreadedRequest(_scenePath);
		var tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Cubic);
		tween.TweenProperty(_logo, "modulate:a", 1.0, 1.2);
		tween.TweenCallback(Callable.From(() => {
			var packedScene = ResourceLoader.LoadThreadedGet(_scenePath) as PackedScene;
			instancedScene = packedScene?.Instantiate<Control>();
			if (instancedScene == null) return;
			instancedScene.Visible = false;
			AddSibling(instancedScene);
			Utils.Tree.Root.MoveChild(instancedScene, -2);
		}));
		tween.TweenCallback(Callable.From(() => { instancedScene!.Visible = true; })).SetDelay(0.05);
		tween.Parallel().TweenProperty(this, "modulate:a", 0, 0.8);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}