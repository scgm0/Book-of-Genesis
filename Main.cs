using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Jint;
using Jint.Constraints;
using Jint.Native;
using Jint.Native.Json;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;
using SourceMaps;
using World;
using Engine = Jint.Engine;
using Environment = System.Environment;
using Timer = System.Timers.Timer;

#pragma warning disable CS8974 // 将方法组转换为非委托类型
#pragma warning disable IL2026
#pragma warning disable IL2111

namespace 创世记;

public sealed partial class Main : Control {
	[GetNode("../BootSplash")] private BootSplash _bootSplash;
	[GetNode("%ChooseWorldButton")] private Button _chooseWorldButton;
	[GetNode("%Home")] private Control _home;
	[GetNode("%GameVersion")] private Label _gameVersion;
	[GetNode("%DotNetVersion")] private Label _dotNetVersion;
	[GetNode("%ChooseWorld")] private Control _chooseWorld;
	[GetNode("%ReadyBar")] private Control _readyBar;
	[GetNode("%TemplateWorldButton")] private Button _templateWorldButton;
	[GetNode("%LogButton")] private Button _logButton;
	[GetNode("%WorldsPathHint")] private LinkButton _worldsPathHint;
	[GetNode("%Background")] private Control _background;
	[GetNode("%Background/ColorRect")] private ColorRect _backgroundColorRect;
	[GetNode("%Background/TextureRect")] private TextureRect _backgroundTextureRect;
	[GetNode("%Back")] private Button _back;

	[Export] private PackedScene _worldScene;
	[Export] private PackedScene _worldItem;

	private World _world;

	static private readonly DateTime StartTime = DateTime.Now;

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

		Log.Debug("启动游戏",
			"\nPlatform:",
			OS.GetName(),
			"\nGameVersion:",
			Utils.GameVersion,
			"\nDotNetVersion:",
			Environment.Version.ToString());

		_gameVersion.Text = $"v{Utils.GameVersion}";
		_dotNetVersion.Text = $"dotnet: {Environment.Version}";
		_worldsPathHint.Text += ProjectSettings.GlobalizePath(Utils.UserWorldsPath);
		_worldsPathHint.Uri = ProjectSettings.GlobalizePath(Utils.UserWorldsPath);

		_chooseWorldButton.Pressed += ChooseWorld;
		_templateWorldButton.Pressed += ChooseTemplate;
		_logButton.Pressed += Log.LogWindow.ToggleVisible;
		_back.Pressed +=
			() => GetTree().Root.PropagateNotification((int)NotificationWMGoBackRequest);

		_bootSplash.TreeExited += _readyBar.Show;
		Task.Run(() => {
			TsTransform.Prepare();
			Log.Debug("初始化完成，耗时:", (DateTime.Now - StartTime).ToString());
			_readyBar.CallDeferred(CanvasItem.MethodName.Hide);
			Utils.Tree.AutoAcceptQuit = false;
		});
	}

	public override void _Input(InputEvent @event) {
		if (!@event.IsActionPressed("ui_select")) return;
		using var pressedEvent = new InputEventMouseButton();
		pressedEvent.ButtonIndex = MouseButton.Left;
		pressedEvent.Pressed = true;
		pressedEvent.Position = GetGlobalMousePosition();
		Input.ParseInputEvent(pressedEvent);
		using var releasedEvent = (InputEventMouseButton)pressedEvent.Duplicate(true);
		releasedEvent.Pressed = false;
		Input.Singleton.CallDeferred(Input.MethodName.ParseInputEvent, releasedEvent);
	}

	public override void _Notification(int what) {
		if (what == NotificationWMCloseRequest) {
			if (CurrentEngine != null) {
				ExitWorld();
			}

			_worldScene.Dispose();
			_worldItem.Dispose();
			Utils.GlobalConfig.Dispose();
			Log.Debug("退出游戏，运行时长:", (DateTime.Now - StartTime).ToString());
			GetTree().Quit();
		} else if (what == NotificationWMGoBackRequest) {
			if (_world is { Visible: true }) {
				ExitWorld();
			} else if (_chooseWorld.Visible) {
				_home.Show();
				_chooseWorld.Hide();
			} else if (_home.Visible) {
				GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
			}
		}
	}

	public override void _PhysicsProcess(double delta) {
		if (_currentWorld == null) return;
		try {
			EmitEvent(EventType.Tick);
		} catch (JavaScriptException e) {
			Log.Error(
				$"{e.Error}\n{StackTraceParser.ReTrace(Utils.SourceMapCollection!, e.JavaScriptStackTrace ?? string.Empty)}");
		} catch (Exception e) {
			CatchExceptions(e);
		}
	}


	private void ChooseWorld() {
		Log.Debug("加载世界列表");
		ClearCache();
		LoadWorldInfos(Utils.UserWorldsPath, true);
		LoadWorldInfos(Utils.ResWorldsPath);
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

			worldItem.GetNode<Button>("%Choose").Pressed += () => LoadWorld(worldInfo);
			worldItem.GetNode<Button>("%Choose").Text = "进入\n世界";
			list.AddChild(worldItem);
		}

		Log.Debug("世界列表加载完成");
	}

	private void ChooseTemplate() {
		Log.Debug("加载模版列表");
		ClearCache();
		LoadWorldInfos(Utils.ResTemplatesPath);
		_home.Hide();
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
				GetTree().Root.PropagateNotification((int)NotificationWMGoBackRequest);
			};
			worldItem.GetNode<Button>("%Choose").Text = "导出\n模版";
			list.AddChild(worldItem);
		}

		Log.Debug("模版列表加载完成");
	}

	private void LoadWorld(WorldInfo worldInfo) {
		CurrentWorldInfo = worldInfo;
		Log.Debug("加载世界:", CurrentWorldInfo.JsonString);
		_home.Hide();
		_chooseWorld.Hide();
		_background.Modulate = Color.FromHtml("#ffffff00");
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
		Task.Run(() => {
			InitEngine();
			this.SyncPost(_ => {
				using var tween = _world.CreateTween();
				tween.SetEase(Tween.EaseType.Out);
				tween.TweenProperty(_background, "modulate:a", 1.5, 1.5);
				tween.TweenCallback(Callable.From(() => {
					if (CurrentWorldInfo == null) return;
					Log.Debug("进入世界:", CurrentWorldInfo.JsonString);
					_world.GetNode<Control>("Main").Show();
					_currentWorldEvent = CurrentEngine!.GetValue("World").Get("event").As<JsObject>()!;
					EmitEvent(EventType.Ready);
					_currentWorld = (JsObject)CurrentEngine.GetValue("World");
				}));
			});

			try {
				CurrentEngine!.Modules.Import(CurrentWorldInfo!.Main);
			} catch (Exception e) {
				CatchExceptions(e);
			}
		});
	}

	private void InitWorld() {
		// ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
		var oldWorld = _world ?? GetNode("%World");
		_world = _worldScene.Instantiate<World>();
		_world.GetNode<Control>("Main").Hide();
		oldWorld.AddSibling(_world);
		oldWorld.QueueFree();

		_world.SetTitle($"{CurrentWorldInfo!.Name}-{CurrentWorldInfo.Version}");
		_world.GetNode<Button>("%Encrypt").Disabled = CurrentWorldInfo.IsEncrypt;
		_world.GetNode<Button>("%Log").Pressed += Log.LogWindow.ToggleVisible;
		_world.GetNode<Button>("%Overload").Pressed += () => {
			var worldKey = CurrentWorldInfo.WorldKey;
			var worldsPath = CurrentWorldInfo.GlobalPath.GetBaseDir();
			var worldConfigPath = CurrentWorldInfo.GlobalPath.PathJoin("config.json");
			var fileName = CurrentWorldInfo.GlobalPath.GetFile();
			var encrypt = CurrentWorldInfo.IsEncrypt;
			ExitWorld();
			DeserializeWorldInfo(worldConfigPath, fileName, worldsPath, !encrypt);
			LoadWorld(Utils.WorldInfos[worldKey]);
		};
		_world.GetNode<Button>("%Exit").Pressed +=
			() => Utils.Tree.Root.PropagateNotification((int)NotificationWMGoBackRequest);
		_world.GetNode<Button>("%Encrypt").Pressed += () => {
			Utils.ExportEncryptionWorldPck(CurrentWorldInfo);
			ExitWorld();
		};

		_world.CommandEdit.TextSubmitted += text => {
			_world.CommandEdit.Text = "";
			EmitEvent(EventType.Command, text);
		};
	}

	private void InitEngine() {
		try {
			Utils.SourceMapCollection = new SourceMapCollection();
			Utils.Tcs = new CancellationTokenSource();
			CurrentEngine = new Engine(options => {
				options.CancellationToken(new CancellationToken(true));
				options.EnableModules(new CustomModuleLoader(CurrentWorldInfo!));
			});
			Utils.JsonParser = new JsonParser(CurrentEngine);
			Utils.JsonSerializer = new JsonSerializer(CurrentEngine);
			var constraint = CurrentEngine.Constraints.Find<CancellationConstraint>();
			constraint?.Reset(Utils.Tcs.Token);

			CurrentEngine.SetValue("print", Log.Info)
				.SetValue("setTimeout", SetTimeout)
				.SetValue("setInterval", SetInterval)
				.SetValue("clearTimeout",
					(int id) => {
						if (!Utils.Timers.TryGetValue(id, out var value)) return;
						value.Stop();
						Utils.Timers.Remove(id);
						value.Dispose();
					})
				.SetValue("clearInterval",
					(int id) => {
						if (!Utils.Timers.TryGetValue(id, out var value)) return;
						value.Stop();
						Utils.Timers.Remove(id);
						value.Dispose();
					});

			CurrentEngine.Modules.Add("events", builder => builder.AddModule(Utils.Polyfill.Events));
			CurrentEngine.Modules.Add("audio",
				builder => builder.ExportType<AudioPlayer>().ExportType<AudioPlayer>("default"));

			var currentWorld = new JsObject(CurrentEngine);
			var worldEvent = CurrentEngine.Construct(CurrentEngine.Modules.Import("events").Get("default"));

			currentWorld.Set("FilterType", TypeReference.CreateTypeReference(CurrentEngine, typeof(FilterType)));
			currentWorld.Set("EventType", TypeReference.CreateTypeReference(CurrentEngine, typeof(EventType)));
			currentWorld.Set("TextType", TypeReference.CreateTypeReference(CurrentEngine, typeof(TextType)));

			currentWorld.DefineOwnProperty("gameVersion",
				new GetSetPropertyDescriptor(
					JsValue.FromObject(CurrentEngine, () => Utils.GameVersion),
					null));

			currentWorld.DefineOwnProperty("info",
				new GetSetPropertyDescriptor(
					JsValue.FromObject(CurrentEngine,
						() => Utils.JsonParser.Parse(CurrentWorldInfo!.JsonString)),
					null));
			currentWorld.DefineOwnProperty("event",
				new GetSetPropertyDescriptor(
					JsValue.FromObject(CurrentEngine, () => worldEvent),
					null));

			currentWorld.Set("setBackgroundColor",
				JsValue.FromObject(CurrentEngine,
					(string colorHex) => this.SyncSend(_ => _backgroundColorRect.Color = Color.FromString(colorHex, Color.Color8(74, 74, 74)))));
			currentWorld.Set("setBackgroundTexture",
				JsValue.FromObject(CurrentEngine,
					(string path, FilterType filter = FilterType.Linear) => this.SyncSend(_ => _backgroundTextureRect.Texture = Utils.LoadImageFile(path, filter))));

			currentWorld.Set("setTitle",
				JsValue.FromObject(CurrentEngine, _world.SetTitle));

			currentWorld.Set("setLeftText",
				JsValue.FromObject(CurrentEngine, _world.SetLeftText));
			currentWorld.Set("setCenterText",
				JsValue.FromObject(CurrentEngine, _world.SetCenterText));
			currentWorld.Set("setRightText",
				JsValue.FromObject(CurrentEngine, _world.SetRightText));
			currentWorld.Set("addLeftText",
				JsValue.FromObject(CurrentEngine, _world.AddLeftText));
			currentWorld.Set("addCenterText",
				JsValue.FromObject(CurrentEngine, _world.AddCenterText));
			currentWorld.Set("addRightText",
				JsValue.FromObject(CurrentEngine, _world.AddRightText));

			currentWorld.Set("setLeftStretchRatio",
				JsValue.FromObject(CurrentEngine, _world.SetLeftStretchRatio));
			currentWorld.Set("setCenterStretchRatio",
				JsValue.FromObject(CurrentEngine, _world.SetCenterStretchRatio));
			currentWorld.Set("setRightStretchRatio",
				JsValue.FromObject(CurrentEngine, _world.SetRightStretchRatio));

			currentWorld.Set("addLeftButton",
				JsValue.FromObject(CurrentEngine, _world.AddLeftButton));
			currentWorld.Set("addRightButton",
				JsValue.FromObject(CurrentEngine, _world.AddRightButton));
			currentWorld.Set("setLeftButtons",
				JsValue.FromObject(CurrentEngine, _world.SetLeftButtons));
			currentWorld.Set("setRightButtons",
				JsValue.FromObject(CurrentEngine, _world.SetRightButtons));
			currentWorld.Set("removeLeftButtonByIndex",
				JsValue.FromObject(CurrentEngine, _world.RemoveLeftButtonByIndex));
			currentWorld.Set("removeRightButtonByIndex",
				JsValue.FromObject(CurrentEngine, _world.RemoveRightButtonByIndex));
			currentWorld.Set("removeButtonById",
				JsValue.FromObject(CurrentEngine, World.RemoveButtonById));

			currentWorld.Set("setCommandPlaceholderText",
				JsValue.FromObject(CurrentEngine, _world.SetCommandPlaceholderText));

			currentWorld.Set("setTextBackgroundColor",
				JsValue.FromObject(CurrentEngine,
					(TextType type, string colorHex) => _world.SetTextBackgroundColor(type, colorHex)));
			currentWorld.Set("setTextFontColor",
				JsValue.FromObject(CurrentEngine,
					(TextType type, string colorHex) => _world.SetTextFontColor(type, colorHex)));

			currentWorld.Set("print", CurrentEngine.GetValue("print"));
			currentWorld.Set("toast", JsValue.FromObject(CurrentEngine, _world.ShowToast));
			currentWorld.Set("getSaveValue",
				JsValue.FromObject(CurrentEngine,
					(string section, string key, JsValue? defaultValue = null) => {
						var value = GetSaveValue(section, key).VariantToJsValue();
						return value == JsValue.Undefined ? defaultValue ?? JsValue.Undefined : value;
					}));
			currentWorld.Set("setSaveValue",
				JsValue.FromObject(CurrentEngine, SetSaveValue));
			currentWorld.Set("getGlobalSaveValue",
				JsValue.FromObject(CurrentEngine,
					(string section, string key, JsValue? defaultValue = null) => {
						var variant = Utils.GlobalConfig.GetValue(section, key, new RefCounted());
						var value = variant.VariantToJsValue();
						return value == JsValue.Undefined ? defaultValue ?? JsValue.Undefined : value;
					}));
			currentWorld.Set("setGlobalSaveValue",
				JsValue.FromObject(CurrentEngine,
					(string section, string key, JsValue value) => {
						Utils.GlobalConfig.SetValue(section, key, value.JsValueToVariant());
						Utils.GlobalConfig.SaveEncryptedPass($"{Utils.SavesPath}/global.save", "global");
					}));

			currentWorld.Set("delay",
				JsValue.FromObject(CurrentEngine, (int delay) => Task.Delay(delay)));

			currentWorld.Set("versionCompare",
				JsValue.FromObject(CurrentEngine, Utils.VersionCompare));

			currentWorld.Set("exit",
				JsValue.FromObject(CurrentEngine, ExitWorld));

			CurrentEngine.SetValue("World", currentWorld);
		} catch (Exception e) {
			Log.Error(e.ToString());
		}
	}

	private int SetTimeout(JsValue callback, int delay, params JsValue[]? values) {
		return AddTimer(false, callback, delay, values);
	}

	private int SetInterval(JsValue callback, int delay, params JsValue[]? values) {
		return AddTimer(true, callback, delay, values);
	}

	private int AddTimer(bool autoReset, JsValue callback, int delay, params JsValue[]? values) {
		if (delay <= 0) {
			callback.Call(thisObj: JsValue.Undefined, values ?? []);
			CurrentEngine!.Advanced.ProcessTasks();
			return 0;
		}

		var timer = new Timer(delay);
		var id = timer.GetHashCode();
		Utils.Timers[id] = timer;
		timer.AutoReset = autoReset;
		timer.Elapsed += (_, _) => {
			if (!Utils.Timers.ContainsKey(id)) {
				timer.Dispose();
				return;
			}

			this.SyncPost(_ => {
				try {
					callback.Call(thisObj: JsValue.Undefined, values ?? []);
					CurrentEngine?.Advanced.ProcessTasks();
				} catch (Exception e) {
					CatchExceptions(e);
				}
			});

			if (autoReset) return;
			Utils.Timers.Remove(id);
			timer.Dispose();
		};
		timer.Enabled = true;
		return id;
	}

	private void ClearCache() {
		foreach (var (_, timer) in Utils.Timers) {
			timer.Stop();
			timer.Dispose();
		}

		Utils.Timers.Clear();
		Utils.Tcs?.Dispose();
		Utils.Tcs = null;
		Utils.SourceMapCollection = null;

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

		_background.Modulate = Colors.White;
		_background.GetNode<ColorRect>("ColorRect").Color = Color.Color8(74, 74, 74);
		_background.GetNode<TextureRect>("TextureRect").Texture = null;
	}

	public void ExitWorld(int exitCode = 0) {
		if (CurrentEngine is null || CurrentWorldInfo is null) return;
		EmitEvent(EventType.Exit, exitCode);
		Utils.Tcs?.Cancel();
		Log.Debug("退出世界:", exitCode.ToString(), CurrentWorldInfo.JsonString);
		CurrentEngine.Dispose();
		CurrentEngine = null;
		_currentWorldEvent = null;
		_currentWorld = null;
		CurrentWorldInfo = null;
		this.SyncSend(_ => {
			ClearCache();
			_home.Show();
			_world.Hide();
		});
	}

	public static void CatchExceptions(Exception exception) {
		if (exception is ExecutionCanceledException) return;
		string str;
		if (exception is JavaScriptException javaScriptException) {
			str = $"{javaScriptException.Error}\n{StackTraceParser.ReTrace(Utils.SourceMapCollection!, javaScriptException.JavaScriptStackTrace ?? string.Empty)}";
		} else {
			str = exception.ToString();
		}

		Log.Error(str);
	}

	public static void OnMetaClickedEventHandler(Variant meta, int index) {
		var json = new Json();
		var value = json.Parse(meta.AsString()) == Error.Ok ? json.Data.VariantToJsValue() : meta.VariantToJsValue();
		EmitEvent(EventType.TextUrlClick, value, index);
	}

	public static void EmitEvent(EventType name, params JsValue[] values) {
		try {
			_currentWorldEvent?["emit"].Call(thisObj: _currentWorldEvent, [(int)name, ..values]);
		} catch (Exception e) {
			CatchExceptions(e);
		}
	}

	static private Variant GetSaveValue(string section, string key) {
		var value = CurrentWorldInfo!.Config.GetValue(section, key, new RefCounted());
		return value;
	}

	static private void SetSaveValue(string section, string key, JsValue value) {
		CurrentWorldInfo!.Config.SetValue(section, key, value.JsValueToVariant());
		CurrentWorldInfo.Config.SaveEncryptedPass(
			$"{Utils.SavesPath}/{CurrentWorldInfo.Author}:{CurrentWorldInfo.Name}.save",
			$"{CurrentWorldInfo.Author}:{CurrentWorldInfo.Name}");
	}
}