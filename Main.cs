using System;
using System.Linq;
using Godot;
using Environment = System.Environment;

#pragma warning disable CS8974 // 将方法组转换为非委托类型
#pragma warning disable IL2026
#pragma warning disable IL2111

namespace 创世记;

public sealed partial class Main : Control {
	[GetNode("%ChooseWorldButton")] private Button _chooseWorldButton;
	[GetNode("%GameVersion")] private Label _gameVersion;
	[GetNode("%DotNetVersion")] private Label _dotNetVersion;
	[GetNode("%ChooseWorld")] private Control _chooseWorld;
	[GetNode("%ReadyBar")] private Control _readyBar;
	[GetNode("%Home")] private Control _home;
	[GetNode("%TemplateWorldButton")] private Button _templateWorldButton;
	[GetNode("%LogButton")] private Button _logButton;
	[GetNode("%ExitButton")] private Button _exitButton;
	[GetNode("%WorldsPathHint")] private LinkButton _worldsPathHint;
	[GetNode("%Back")] private Button _back;

	[Export] private PackedScene _worldScene;
	[Export] private PackedScene _worldItem;

	private World? _world;

	static private readonly DateTime StartTime;

	static Main() {
		StartTime = DateTime.Now;
	}

	public override void _Ready() {
		if (!DirAccess.DirExistsAbsolute(Utils.UserWorldsPath)) {
			DirAccess.MakeDirRecursiveAbsolute(Utils.UserWorldsPath);
		}

		if (!DirAccess.DirExistsAbsolute(Utils.SavesPath)) {
			DirAccess.MakeDirRecursiveAbsolute(Utils.SavesPath);
		}

		if (!DirAccess.DirExistsAbsolute(Utils.TsGenPath)) {
			DirAccess.MakeDirRecursiveAbsolute(Utils.TsGenPath);
		}

		ProjectSettings.LoadResourcePack(Utils.TemplateZipPath);

		_gameVersion.Text = $"v{Utils.GameVersion}";
		_dotNetVersion.Text = $"dotnet: {Environment.Version}";
		_worldsPathHint.Text += ProjectSettings.GlobalizePath(Utils.UserWorldsPath).SimplifyPath();
		_worldsPathHint.Uri = ProjectSettings.GlobalizePath(Utils.UserWorldsPath).SimplifyPath();

		_chooseWorldButton.Pressed += ChooseWorld;
		_templateWorldButton.Pressed += ChooseTemplate;
		_logButton.Pressed += Log.LogWindow.ToggleVisible;
		_exitButton.Pressed += () => Utils.Tree.Root.PropagateNotification((int)NotificationWMCloseRequest);
		_back.Pressed += () => Utils.Tree.Root.PropagateNotification((int)NotificationWMGoBackRequest);

		TsTransform.Prepare();
		Log.Debug($"初始化完成，耗时: {(DateTime.Now - StartTime).ToString()}\nPlatform: {OS.GetName()}\nGameVersion: {Utils.GameVersion}\nDotNetVersion: {Environment.Version}\nTypeScriptVersion: {TsTransform.TypeScriptVersion}");
		Utils.Tree.AutoAcceptQuit = false;
	}

	public override void _Input(InputEvent @event) {
		using (@event) {
			if (!@event.IsActionPressed("ui_accept")) return;
			using var pressedEvent = new InputEventMouseButton();
			pressedEvent.ButtonIndex = MouseButton.Left;
			pressedEvent.Pressed = true;
			pressedEvent.Position = GetViewportTransform() * GetGlobalTransformWithCanvas() * GetLocalMousePosition();
			Input.ParseInputEvent(pressedEvent);
			using var releasedEvent = (InputEventMouseButton)pressedEvent.Duplicate(true);
			releasedEvent.Pressed = false;
			Input.Singleton.CallDeferred(Input.MethodName.ParseInputEvent, releasedEvent);
		}
	}

	public override void _Notification(int what) {
		if (what == NotificationWMCloseRequest) {
			World.Instance?.Exit();
			_worldScene.Dispose();
			_worldItem.Dispose();
			Utils.GlobalConfig.Dispose();
			Log.Debug("退出游戏，运行时长:", (DateTime.Now - StartTime).ToString());
			Utils.Tree.Quit();
		} else if (what == NotificationWMGoBackRequest) {
			if (_world is { Visible: true }) {
				World.Instance?.Exit();
			} else if (_chooseWorld.Visible) {
				_chooseWorld.Hide();
			} else {
				Utils.Tree.Root.PropagateNotification((int)NotificationWMCloseRequest);
			}
		}
	}

	private async void ChooseWorld() {
		Log.Debug("加载世界列表");
		ClearCache();
		Utils.LoadPacks(Utils.UserWorldsPath);
		LoadWorldInfos(Utils.UserWorldsPath);
		LoadWorldInfos(Utils.ResWorldsPath, true);
		_chooseWorldButton.ReleaseFocus();
		_chooseWorld.Show();
		var list = GetNode<VBoxContainer>("%WorldList");
		foreach (var child in list.GetChildren()) {
			child.QueueFree();
		}

		foreach (var (key, worldInfo) in Utils.WorldInfos.OrderByDescending(worldInfo => worldInfo.Value.WorldModifiedTime)) {
			Log.Debug(key, worldInfo.JsonString);
			var worldItem = _worldItem.Instantiate();
			worldItem.GetNode<Label>("%Name").Text = $"{worldInfo.Name}-{worldInfo.Version}";
			worldItem.GetNode<Label>("%Description").Text = $"{worldInfo.Author}\n{worldInfo.Description}";
			worldItem.GetNode<TextureRect>("%Encrypt").Visible = worldInfo.IsEncrypt;
			var icon = Utils.LoadImageFile(worldInfo, worldInfo.Icon);
			if (icon != null) {
				worldItem.GetNode<TextureRect>("%Icon").Texture = icon;
			}

			worldItem.GetNode<Button>("%Choose").Pressed += () => LoadWorld(worldInfo);
			worldItem.GetNode<Button>("%Choose").Text = "进入\n世界";
			worldItem.Set("modulate", Colors.Transparent);
			list.AddChild(worldItem);
			using var tween = worldItem.CreateTween();
			tween.TweenProperty(worldItem, "modulate:a", 1, 0.125f);
			await ToSignal(tween, Tween.SignalName.Finished);
		}

		Log.Debug("世界列表加载完成");
	}

	private async void ChooseTemplate() {
		Log.Debug("加载模版列表");
		ClearCache();
		LoadWorldInfos(Utils.ResTemplatesPath);
		_templateWorldButton.ReleaseFocus();
		_chooseWorld.Show();
		var list = GetNode<VBoxContainer>("%WorldList");
		foreach (var child in list.GetChildren()) {
			child.QueueFree();
		}

		foreach (var (key, worldInfo) in Utils.WorldInfos) {
			Log.Debug(key, worldInfo.JsonString);
			var worldItem = _worldItem.Instantiate();
			worldItem.GetNode<Label>("%Name").Text = $"{worldInfo.Name}-{worldInfo.Version}";
			worldItem.GetNode<Label>("%Description").Text = $"{worldInfo.Author}\n{worldInfo.Description}";
			worldItem.GetNode<TextureRect>("%Encrypt").Visible = worldInfo.IsEncrypt;
			var icon = Utils.LoadImageFile(worldInfo, worldInfo.Icon);
			if (icon != null) {
				worldItem.GetNode<TextureRect>("%Icon").Texture = icon;
			}

			worldItem.GetNode<Button>("%Choose").Pressed += () => {
				var exportPath = Utils.UserWorldsPath.PathJoin($"{worldInfo.Name}-{worldInfo.Version}");
				Log.Debug("导出模版:", worldInfo.JsonString);
				Utils.CopyDir(worldInfo.GlobalPath, Utils.UserWorldsPath.PathJoin($"{worldInfo.Name}-{worldInfo.Version}"));
				Log.Debug("导出模版完成:", exportPath);
				Utils.Tree.Root.PropagateNotification((int)NotificationWMGoBackRequest);
			};
			worldItem.GetNode<Button>("%Choose").Text = "导出\n模版";
			worldItem.Set("modulate", Colors.Transparent);
			list.AddChild(worldItem);
			using var tween = worldItem.CreateTween();
			tween.TweenProperty(worldItem, "modulate:a", 1, 0.125f);
			await ToSignal(tween, Tween.SignalName.Finished);
		}

		Log.Debug("模版列表加载完成");
	}

	private void LoadWorld(WorldInfo worldInfo) {
		if (FileAccess.GetFileAsString(worldInfo.GlobalPath.PathJoin("./config.json")).Length <= 1) {
			Utils.Tree.Root.PropagateNotification((int)NotificationWMGoBackRequest);
			Log.Error("世界不存在:", worldInfo.JsonString);
			return;
		}

		CurrentWorldInfo = worldInfo;
		Log.Debug("加载世界:", CurrentWorldInfo.JsonString);
		_chooseWorld.Hide();
		CurrentWorldInfo.Config.LoadEncryptedPass(
			$"{Utils.SavesPath}/{CurrentWorldInfo.Author}:{CurrentWorldInfo.Name}.save",
			$"{CurrentWorldInfo.Author}:{CurrentWorldInfo.Name}");
		CurrentWorldInfo.Config.SaveEncryptedPass(
			$"{Utils.SavesPath}/{CurrentWorldInfo.Author}:{CurrentWorldInfo.Name}.save",
			$"{CurrentWorldInfo.Author}:{CurrentWorldInfo.Name}");
		Utils.GlobalConfig.LoadEncryptedPass($"{Utils.SavesPath}/global.save", "global");
		Utils.GlobalConfig.SaveEncryptedPass($"{Utils.SavesPath}/global.save", "global");
		RunWorld();
	}

	private void RunWorld() {
		InitWorld();
		if (_world == null) return;
		using var tween = _world.CreateTween();
		tween.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(_world.BackgroundColor, "modulate:a", 1, 1.5).From(0);
		tween.TweenCallback(new Callable(this, nameof(ReadyWorld)));

	}

	private void ReadyWorld() {
		if (CurrentWorldInfo == null) return;
		Log.Debug("进入世界:", CurrentWorldInfo.JsonString);
		_world?.EventEmit(EventType.Ready);
		_world?.GetNode<Control>("Main").Show();
	}

	private void InitWorld() {
		_world = _worldScene.Instantiate<World>();
		_world.GetNode<Button>("%Overload").Pressed += () => {
			var worldKey = CurrentWorldInfo!.WorldKey;
			var worldsPath = CurrentWorldInfo.GlobalPath.GetBaseDir();
			var worldConfigPath = CurrentWorldInfo.GlobalPath.PathJoin("config.json");
			var fileName = CurrentWorldInfo.GlobalPath.GetFile();
			var encrypt = CurrentWorldInfo.IsEncrypt;
			World.Instance?.Exit();
			DeserializeWorldInfo(worldConfigPath, fileName, worldsPath, encrypt);
			LoadWorld(Utils.WorldInfos[worldKey]);
		};

		_home.AddSibling(_world);
	}

	public static void ClearCache() {
		foreach (var audioPlayer in Utils.AudioPlayerCache) {
			audioPlayer.Dispose();
		}

		Utils.AudioPlayerCache.Clear();

		foreach (var (_, worldInfo) in Utils.WorldInfos) {
			worldInfo.Config.Dispose();
		}

		Utils.WorldInfos.Clear();

		foreach (var texture2D in Utils.TextureCache) {
			texture2D.DiffuseTexture?.Dispose();
			texture2D.DiffuseTexture = null;
			texture2D.TakeOverPath(null);
			texture2D.Dispose();
		}

		Utils.TextureCache.Clear();
	}
}
