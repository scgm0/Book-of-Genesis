using System;
using System.Linq;
using System.Text.RegularExpressions;
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
	[GetNode("%WorldsPathHint")] private LinkButton _worldsPathHint;
	[GetNode("%Background")] private Control _background;
	[GetNode("%Background/ColorRect")] private ColorRect _backgroundColorRect;
	[GetNode("%Background/TextureRect")] private TextureRect _backgroundTextureRect;
	[GetNode("%Back")] private Button _back;

	[Export] private PackedScene _worldScene;
	[Export] private PackedScene _worldItem;

	private World _world;
	static private JsonParser? _jsonParser;
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

		Log("启动游戏",
			"\nPlatform:",
			OS.GetName(),
			"\nGameVersion:",
			Utils.GameVersion,
			"\nDotNetVersion:",
			Environment.Version);

		_gameVersion.Text = $"v{Utils.GameVersion}";
		_dotNetVersion.Text = $"dotnet: {Environment.Version}";
		_worldsPathHint.Text += ProjectSettings.GlobalizePath(Utils.UserWorldsPath);
		_worldsPathHint.Uri = ProjectSettings.GlobalizePath(Utils.UserWorldsPath);

		_chooseWorldButton.Pressed += ChooseWorld;
		_templateWorldButton.Pressed += ChooseTemplate;
		_back.Pressed +=
			() => GetTree().Root.PropagateNotification((int)NotificationWMGoBackRequest);

		_bootSplash.TreeExited += _readyBar.Show;
		Task.Run(() => {
			TsTransform.Prepare();
			Log("初始化完成，耗时:", DateTime.Now - StartTime);
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
			Log("退出游戏，运行时长:", DateTime.Now - StartTime);
			GetTree().Quit();
		} else if (what == NotificationWMGoBackRequest) {
			if (_home.Visible) {
				GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
			} else if (_chooseWorld.Visible) {
				_home.Show();
				_chooseWorld.Hide();
			} else if (_world.Visible) {
				ExitWorld();
			}
		}
	}

	public override void _PhysicsProcess(double delta) {
		if (_currentWorld == null) return;
		try {
			EmitEvent(EventType.Tick);
		} catch (JavaScriptException e) {
			Log(
				$"{e.Error}\n{StackTraceParser.ReTrace(Utils.SourceMapCollection!, e.JavaScriptStackTrace ?? string.Empty)}");
			ExitWorld(1);
		} catch (Exception e) {
			if (e is ExecutionCanceledException) {
				return;
			}

			Log(e.ToString());
			ExitWorld(1);
		}
	}


	private void ChooseWorld() {
		Log("加载世界列表");
		ClearCache();
		LoadWorldInfos(Utils.UserWorldsPath, true);
		LoadWorldInfos(Utils.ResWorldsPath);
		_home.Hide();
		_chooseWorld.Show();
		var list = GetNode<VBoxContainer>("%WorldList");
		foreach (var child in list.GetChildren()) {
			child.QueueFree();
		}

		foreach (var (key, worldInfo) in Utils.WorldInfos) {
			Log(key, worldInfo.JsonString);
			var worldItem = _worldItem.Instantiate();
			worldItem.GetNode<Label>("%Name").Text = $"{worldInfo.Name}-{worldInfo.Version}";
			worldItem.GetNode<Label>("%Description").Text = $"{worldInfo.Author}\n{worldInfo.Description}";
			worldItem.GetNode<TextureRect>("%Encrypt").Visible = worldInfo.IsEncrypt;
			var icon = LoadImageFile(worldInfo, worldInfo.Icon);
			if (icon != null) {
				worldItem.GetNode<TextureRect>("%Icon").Texture = icon;
			}

			worldItem.GetNode<Button>("%Choose").Pressed += () => {
				CurrentWorldInfo = worldInfo;
				Log("选择世界:", CurrentWorldInfo.JsonString);
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
			};
			worldItem.GetNode<Button>("%Choose").Text = "进入\n世界";
			list.AddChild(worldItem);
		}

		Log("世界列表加载完成");
	}

	private void ChooseTemplate() {
		Log("加载模版列表");
		ClearCache();
		LoadWorldInfos(Utils.ResTemplatesPath);
		_home.Hide();
		_chooseWorld.Show();
		var list = GetNode<VBoxContainer>("%WorldList");
		foreach (var child in list.GetChildren()) {
			child.QueueFree();
		}

		foreach (var (key, worldInfo) in Utils.WorldInfos) {
			Log(key, worldInfo.JsonString);
			var worldItem = _worldItem.Instantiate();
			worldItem.GetNode<Label>("%Name").Text = $"{worldInfo.Name}-{worldInfo.Version}";
			worldItem.GetNode<Label>("%Description").Text = $"{worldInfo.Author}\n{worldInfo.Description}";
			worldItem.GetNode<TextureRect>("%Encrypt").Visible = worldInfo.IsEncrypt;
			var icon = LoadImageFile(worldInfo, worldInfo.Icon);
			if (icon != null) {
				worldItem.GetNode<TextureRect>("%Icon").Texture = icon;
			}

			worldItem.GetNode<Button>("%Choose").Pressed += () => {
				var exportPath = Utils.UserWorldsPath.PathJoin($"{worldInfo.Name}-{worldInfo.Version}");
				Log("导出模版:", worldInfo.JsonString);
				Utils.CopyDir(worldInfo.GlobalPath, Utils.UserWorldsPath.PathJoin($"{worldInfo.Name}-{worldInfo.Version}"));
				Log("导出模版完成:", exportPath);
				GetTree().Root.PropagateNotification((int)NotificationWMGoBackRequest);
			};
			worldItem.GetNode<Button>("%Choose").Text = "导出\n模版";
			list.AddChild(worldItem);
		}

		Log("模版列表加载完成");
	}

	private void RunWorld() {
		try {
			InitWorld();
			InitEngine();
			CurrentEngine!.Modules.Import(CurrentWorldInfo!.Main);
			using var tween = _world.CreateTween();
			tween.SetEase(Tween.EaseType.Out);
			tween.TweenProperty(_background, "modulate:a", 1.5, 1.5);
			tween.TweenCallback(Callable.From(() => {
				Log("进入世界:", CurrentWorldInfo.JsonString);
				_world.GetNode<Control>("Main").Show();
				_currentWorldEvent = CurrentEngine.GetValue("World").Get("event").As<JsObject>()!;
				EmitEvent(EventType.Ready);
				_currentWorld = (JsObject)CurrentEngine.GetValue("World");
			}));
		} catch (JavaScriptException e) {
			Log(
				$"{e.Error}\n{StackTraceParser.ReTrace(Utils.SourceMapCollection!, e.JavaScriptStackTrace ?? string.Empty)}");
			ExitWorld(1);
		}
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
			_jsonParser = new JsonParser(CurrentEngine);
			var constraint = CurrentEngine.Constraints.Find<CancellationConstraint>();
			constraint?.Reset(Utils.Tcs.Token);

			CurrentEngine.SetValue("print", new Action<string[]>(Log))
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

			CurrentEngine.Modules.Add("events", Utils.Polyfill.Events);
			CurrentEngine.Modules.Add("audio",
				builder => builder.ExportType<AudioPlayer>().ExportType<AudioPlayer>("default"));

			var currentWorld = new JsObject(CurrentEngine);
			var worldEvent = CurrentEngine.Construct(CurrentEngine.Modules.Import("events").Get("default"));

			currentWorld.Set("EventType", TypeReference.CreateTypeReference(CurrentEngine, typeof(EventType)));
			currentWorld.Set("TextType", TypeReference.CreateTypeReference(CurrentEngine, typeof(TextType)));

			currentWorld.DefineOwnProperty("gameVersion",
				new GetSetPropertyDescriptor(
					JsValue.FromObject(CurrentEngine, () => Utils.GameVersion),
					null));

			currentWorld.DefineOwnProperty("info",
				new GetSetPropertyDescriptor(
					JsValue.FromObject(CurrentEngine,
						() => _jsonParser.Parse(CurrentWorldInfo!.JsonString)),
					null));
			currentWorld.DefineOwnProperty("event",
				new GetSetPropertyDescriptor(
					JsValue.FromObject(CurrentEngine, () => worldEvent),
					null));

			currentWorld.Set("setBackgroundColor",
				JsValue.FromObject(CurrentEngine,
					(string colorHex) => {
						var color = Color.FromString(colorHex, Color.Color8(74, 74, 74));
						_backgroundColorRect.Color = color;
					}));
			currentWorld.Set("setBackgroundTexture",
				JsValue.FromObject(CurrentEngine,
					(string path, TextureFilterEnum filter = TextureFilterEnum.Nearest) => {
						var texture = LoadImageFile(path, filter);
						if (_backgroundTextureRect.Texture == texture) return;
						_backgroundTextureRect.Texture = texture;
					}));

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

			currentWorld.FastSetProperty("print",
				new PropertyDescriptor(JsValue.FromObject(CurrentEngine, new Action<string[]>(Log)), true, false, true));
			currentWorld.Set("getSaveValue",
				JsValue.FromObject(CurrentEngine,
					(string section, string key, JsValue? defaultValue = null) => {
						var value = GetSaveValue(section, key).VariantToJsValue(CurrentEngine);
						return value == JsValue.Undefined ? defaultValue ?? JsValue.Undefined : value;
					}));
			currentWorld.Set("setSaveValue",
				JsValue.FromObject(CurrentEngine, SetSaveValue));
			currentWorld.Set("getGlobalSaveValue",
				JsValue.FromObject(CurrentEngine,
					(string section, string key, JsValue? defaultValue = null) => {
						using var defaultGodotObject = new GodotObject();
						var variant = Utils.GlobalConfig.GetValue(section, key, defaultGodotObject);
						if (variant.Obj != defaultGodotObject) defaultGodotObject.Free();
						var value = variant.VariantToJsValue(CurrentEngine);
						return value == JsValue.Undefined ? defaultValue ?? JsValue.Undefined : value;
					}));
			currentWorld.Set("setGlobalSaveValue",
				JsValue.FromObject(CurrentEngine,
					(string section, string key, JsValue value) => {
						Utils.GlobalConfig.SetValue(section, key, value.JsValueToVariant(CurrentEngine));
						Utils.GlobalConfig.SaveEncryptedPass($"{Utils.SavesPath}/global.save", "global");
					}));

			currentWorld.Set("versionCompare",
				JsValue.FromObject(CurrentEngine, Utils.VersionCompare));

			currentWorld.Set("exit",
				JsValue.FromObject(CurrentEngine, ExitWorld));

			CurrentEngine.SetValue("World", currentWorld);
		} catch (Exception e) {
			Log(e.ToString());
			ExitWorld(1);
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
				} catch (JavaScriptException e) {
					Log(
						$"{e.Error}\n{StackTraceParser.ReTrace(Utils.SourceMapCollection!, e.JavaScriptStackTrace ?? string.Empty)}");
					ExitWorld(1);
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
		Utils.Tcs?.Cancel();
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

	private void ExitWorld(int exitCode = 0) {
		Log("退出世界:", exitCode, CurrentWorldInfo?.JsonString ?? string.Empty);
		EmitEvent(EventType.Exit, exitCode);
		CurrentEngine?.Dispose();
		CurrentEngine = null;
		_currentWorldEvent = null;
		_currentWorld = null;
		CurrentWorldInfo = null;

		ClearCache();

		_home.Show();
		_world.Hide();
	}

	public static void OnMetaClickedEventHandler(Variant meta, int index) {
		try {
			EmitEvent(EventType.TextUrlClick, _jsonParser!.Parse(meta.ToString()), index);
		} catch (Exception) {
			EmitEvent(EventType.TextUrlClick, meta.ToString(), index);
		}
	}

	public static void EmitEvent(EventType name, params JsValue[] values) {
		_currentWorldEvent?["emit"].Call(thisObj: _currentWorldEvent, [(int)name, ..values]);
	}

	static private CanvasTexture? LoadImageFile(string path, TextureFilterEnum filter = TextureFilterEnum.Linear) {
		return LoadImageFile(CurrentWorldInfo!, path, filter);
	}

	static private CanvasTexture? LoadImageFile(
		WorldInfo worldInfo,
		string path,
		TextureFilterEnum filter = TextureFilterEnum.Linear) {
		var filePath = worldInfo.GlobalPath.PathJoin(path).SimplifyPath();
		if (!FileAccess.FileExists(filePath)) return null;
		ImageTexture? imageTexture = null;
		if (ResourceLoader.Exists(filePath)) {
			var canvasTexture = GD.Load<CanvasTexture>(filePath);
			if (canvasTexture.TextureFilter == filter) {
				return GD.Load<CanvasTexture>(filePath);
			}

			imageTexture = canvasTexture.DiffuseTexture as ImageTexture;
		}

		var texture = new CanvasTexture();
		texture.TextureFilter = filter;
		texture.TakeOverPath(filePath);
		Utils.TextureCache.Add(texture);
		if (imageTexture == null) {
			var data = FileAccess.GetFileAsBytes(filePath);
			using var img = ImageFromBuffer(data);
			imageTexture = ImageTexture.CreateFromImage(img);
		}

		texture.DiffuseTexture = imageTexture;

		return texture;
	}

	static private Image ImageFromBuffer(byte[] data) {
		var img = new Image();
		switch (ImageFileFormatFinder.GetImageFormat(data)) {
			case ImageFormat.Png:
				img.LoadPngFromBuffer(data);
				break;
			case ImageFormat.Jpg:
				img.LoadJpgFromBuffer(data);
				break;
			case ImageFormat.Bmp:
				img.LoadBmpFromBuffer(data);
				break;
			case ImageFormat.Webp:
				img.LoadWebpFromBuffer(data);
				break;
			case ImageFormat.Unknown:
			default:
				throw new JavaScriptException("不支持的图像格式，仅支持png、jpg、bmp与webp");
		}

		return img;
	}

	static private Variant GetSaveValue(string section, string key) {
		using var defaultValue = new GodotObject();
		var value = CurrentWorldInfo!.Config.GetValue(section, key, defaultValue);
		if (value.Obj != defaultValue) defaultValue.Free();
		return value;
	}

	static private void SetSaveValue(string section, string key, JsValue value) {
		CurrentWorldInfo!.Config.SetValue(section, key, value.JsValueToVariant(CurrentEngine!));
		CurrentWorldInfo.Config.SaveEncryptedPass(
			$"{Utils.SavesPath}/{CurrentWorldInfo.Author}:{CurrentWorldInfo.Name}.save",
			$"{CurrentWorldInfo.Author}:{CurrentWorldInfo.Name}");
	}

	public static void SetRichText(RichTextLabel label, string text) {
		LoadRichTextImg(ref text);
		label.ParseBbcode(text);
	}

	public static void AddRichText(RichTextLabel label, string text) {
		LoadRichTextImg(ref text);
		label.AppendText(text);
	}

	static private void LoadRichTextImg(ref string text) {
		if (string.IsNullOrEmpty(text)) return;
		foreach (Match match in Utils.ImgPathRegex().Matches(text)) {
			var path = match.Groups["path"].Value;
			var oldText = text.Substring(match.Index, match.Length);
			var filter = Utils.ParseExpressionsForValues(oldText);

			var texture = filter switch {
				"linear" => LoadImageFile(path),
				"nearest" => LoadImageFile(path, TextureFilterEnum.Nearest),
				_ => LoadImageFile(path)
			};

			if (texture != null) {
				text = text.Replace(oldText, oldText.Replace(path, texture.ResourcePath));
			}
		}
	}

	public static void Log(params object[] objects) { Log(string.Join(" ", objects.Select(o => o.ToString()))); }

	public static void Log(params string[] strings) { Log(strings.Join(" ")); }

	public static void Log(string str) {
		GD.Print(
			$"[{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}]{(CurrentWorldInfo == null ? " " : $" [{CurrentWorldInfo.Name}] ")}{str}");
	}
}