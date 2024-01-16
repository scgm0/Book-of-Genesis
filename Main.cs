using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Godot;
using Jint;
using Jint.Constraints;
using Jint.Native;
using Jint.Native.Json;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;
using SourceMaps;
using Engine = Jint.Engine;
using Environment = System.Environment;
using Timer = System.Timers.Timer;

namespace 创世记;

public sealed partial class Main : Control {
	[GetNode("%ChooseWorldButton")] private Button _chooseWorldButton;
	[GetNode("%Home")] private Control _home;
	[GetNode("%ChooseWorld")] private Control _chooseWorld;
	[GetNode("%TemplateWorldButton")] private Button _templateWorldButton;
	[GetNode("%Background")] private Control _background;
	[GetNode("%Game")] private Control _game;
	[GetNode("%Game/%LeftButtonList")] private Control _leftButtonList;
	[GetNode("%Game/%RightButtonList")] private Control _rightButtonList;
	[GetNode("%Game/%LeftText")] private RichTextLabel _leftText;
	[GetNode("%Game/%CenterText")] private RichTextLabel _centerText;
	[GetNode("%Game/%RightText")] private RichTextLabel _rightText;
	[GetNode("%Game/%CommandEdit")] private LineEdit _commandEdit;

	[Export] private PackedScene _gameScene;
	[Export] private PackedScene _worldItem;

	static private JsonParser _jsonParser;

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

		Log("游戏开始",
			"\nPlatform:",
			OS.GetName(),
			"\nGameVersion:",
			Utils.GameVersion,
			"\nDotNetVersion:",
			Environment.Version.ToString());

		GetTree().AutoAcceptQuit = false;

		_home.GetNode<LinkButton>("ModsPathHint").Text =
			$"世界存放位置：{ProjectSettings.GlobalizePath(Utils.UserWorldsPath.SimplifyPath())}";
		_home.GetNode<LinkButton>("ModsPathHint").Uri =
			$"{ProjectSettings.GlobalizePath(Utils.UserWorldsPath.SimplifyPath())}";
		_chooseWorldButton.Pressed += ChooseWorld;
		_templateWorldButton.Pressed += ChooseTemplate;
		GetNode<Button>("Window/ChooseWorld/Back").Pressed +=
			() => GetTree().Root.PropagateNotification((int)NotificationWMGoBackRequest);

		TsTransform.Prepare();
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

			_gameScene.Dispose();
			_worldItem.Dispose();
			Utils.GlobalConfig.Dispose();
			GC.Collect();
			GetTree().Quit();
		} else if (what == NotificationWMGoBackRequest) {
			if (_home.Visible) {
				GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
			} else if (_chooseWorld.Visible) {
				_home.Visible = true;
				_chooseWorld.Visible = false;
			} else if (_game.Visible) {
				ExitWorld();
			}
		}
	}

	public override void _PhysicsProcess(double delta) {
		if (_currentWorld == null) return;
		try {
			EmitEvent(WorldEventType.Tick);
		} catch (JavaScriptException e) {
			Log($"{e.Error}\n{StackTraceParser.ReTrace(Utils.SourceMapCollection, e.JavaScriptStackTrace ?? string.Empty)}");
			ExitWorld(1);
		} catch (Exception e) {
			if (e is ExecutionCanceledException) {
				return;
			}

			Log(e.ToString());
			ExitWorld(1);
		}
	}

	private void OnMetaClickedEventHandler(Variant meta, int index) {
		try {
			EmitEvent(WorldEventType.TextUrlClick, _jsonParser.Parse(meta.ToString()), index);
		} catch (Exception) {
			EmitEvent(WorldEventType.TextUrlClick, meta.ToString(), index);
		}
	}

	private void ChooseWorld() {
		Log("加载世界列表");
		ClearCache();
		LoadWorldInfos(Utils.UserWorldsPath, true);
		LoadWorldInfos(Utils.ResWorldsPath);
		_home.Visible = false;
		_chooseWorld.Visible = true;
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
				Log("进入世界:", CurrentWorldInfo.JsonString);
				_chooseWorld.Visible = false;
				_background.Modulate = Color.FromHtml("#ffffff00");
				_game.Visible = true;
				_game.GetNode<RichTextLabel>("%Title").Text = $"{worldInfo.Name}-{worldInfo.Version}";
				_game.GetNode<Button>("%Encrypt").Disabled = worldInfo.IsEncrypt;
				worldInfo.Config.LoadEncryptedPass($"{Utils.SavesPath}/{worldInfo.Author}:{worldInfo.Name}.save",
					$"{worldInfo.Author}:{worldInfo.Name}");
				worldInfo.Config.SaveEncryptedPass($"{Utils.SavesPath}/{worldInfo.Author}:{worldInfo.Name}.save",
					$"{worldInfo.Author}:{worldInfo.Name}");
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
		_home.Visible = false;
		_chooseWorld.Visible = true;
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
			InitGame();
			InitEngine();
			CurrentEngine.Modules.Import(CurrentWorldInfo.Main);
			using var tween = _game.CreateTween();
			tween.SetEase(Tween.EaseType.Out);
			tween.Parallel().TweenProperty(_game, "modulate:a", 1.5, 1.5);

			tween.Parallel().TweenProperty(_background, "modulate:a", 1.5, 1.5);

			tween.TweenCallback(Callable.From(() => {
				_game.GetNode<Control>("Main").Visible = true;
				_currentWorldEvent = CurrentEngine.GetValue("World").Get("event").As<JsObject>()!;
				EmitEvent(WorldEventType.Ready);
				_currentWorld = (JsObject)CurrentEngine?.GetValue("World");
			}));
		} catch (JavaScriptException e) {
			Log($"{e.Error}\n{StackTraceParser.ReTrace(Utils.SourceMapCollection, e.JavaScriptStackTrace ?? string.Empty)}");
			ExitWorld(1);
		}
	}

	private void InitGame() {
		var oldGame = _game;
		_game = _gameScene.Instantiate<Control>();
		_game.GetNode<Control>("Main").Visible = false;
		oldGame.AddSibling(_game);
		oldGame.GetParent().RemoveChild(oldGame);
		oldGame.QueueFree();
		_game.Name = "Game";
		_leftButtonList = _game.GetNode<Control>("%LeftButtonList");
		_rightButtonList = _game.GetNode<Control>("%RightButtonList");
		_leftText = _game.GetNode<RichTextLabel>("%LeftText");
		_centerText = _game.GetNode<RichTextLabel>("%CenterText");
		_rightText = _game.GetNode<RichTextLabel>("%RightText");
		_commandEdit = _game.GetNode<LineEdit>("%CommandEdit");

		_game.GetNode<Button>("%Exit").Pressed += () => GetTree().Root.PropagateNotification((int)NotificationWMGoBackRequest);
		_game.GetNode<Button>("%Encrypt").Pressed += () => {
			Utils.ExportEncryptionWorldPck(CurrentWorldInfo);
			ExitWorld();
		};

		_commandEdit.TextSubmitted += text => {
			_commandEdit.Text = "";
			EmitEvent(WorldEventType.Command, text);
		};

		_leftText.MetaClicked += meta => OnMetaClickedEventHandler(meta, 0);
		_centerText.MetaClicked += meta => OnMetaClickedEventHandler(meta, 1);
		_rightText.MetaClicked += meta => OnMetaClickedEventHandler(meta, 2);

		_leftText.Resized += async () => {
			if (_leftText.GetChildCount() <= 0) return;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			_leftText.GetNode<SmoothScroll>("../..").ScrollToLeft(0);
		};
		_leftText.VisibilityChanged += async () => {
			if (_leftText.GetChildCount() <= 0) return;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			_leftText.GetNode<SmoothScroll>("../..").ScrollToLeft(0);
		};
		_rightButtonList.Resized += async () => {
			if (_rightButtonList.GetChildCount() <= 0) return;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			_rightButtonList.GetNode<SmoothScroll>("../..").ScrollToRight(0);
		};
		_rightButtonList.VisibilityChanged += async () => {
			if (_rightButtonList.GetChildCount() <= 0) return;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			_rightButtonList.GetNode<SmoothScroll>("../..").ScrollToRight(0);
		};
	}

	private void InitEngine() {
		try {
			Utils.SourceMapCollection = new SourceMapCollection();
			Utils.Tcs = new CancellationTokenSource();
			CurrentEngine = new Engine(options => {
				options.CancellationToken(new CancellationToken(true));
				options.EnableModules(new CustomModuleLoader(CurrentWorldInfo));
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
						value?.Stop();
						Utils.Timers.Remove(id);
						value?.Dispose();
					})
				.SetValue("clearInterval",
					(int id) => {
						if (!Utils.Timers.TryGetValue(id, out var value)) return;
						value?.Stop();
						Utils.Timers.Remove(id);
						value?.Dispose();
					});

			CurrentEngine.Modules.Add("events", Utils.Polyfill.Events);
			CurrentEngine.Modules.Add("audio", builder => builder.ExportType<AudioPlayer>().ExportType<AudioPlayer>("default"));

			var currentWorld = new JsObject(CurrentEngine);
			currentWorld.FastSetProperty("print",
				new PropertyDescriptor(new DelegateWrapper(CurrentEngine, new Action<string[]>(Log)), true, false, true));
			var worldEvent = CurrentEngine.Construct(CurrentEngine.Modules.Import("events").Get("default"));
			currentWorld.DefineOwnProperty("event",
				new GetSetPropertyDescriptor(
					new DelegateWrapper(CurrentEngine, () => worldEvent),
					null));
			currentWorld.DefineOwnProperty("info",
				new GetSetPropertyDescriptor(
					new DelegateWrapper(CurrentEngine,
						() => _jsonParser.Parse(CurrentWorldInfo.JsonString)),
					null));
			currentWorld.Set("setBackgroundColor",
				new DelegateWrapper(CurrentEngine,
					(string color) => {
						_background.GetNode<ColorRect>("ColorRect").Color =
							Color.FromString(color, Color.Color8(74, 74, 74)) with { A = 1 };
					}));
			currentWorld.Set("setBackgroundTexture",
				new DelegateWrapper(CurrentEngine,
					(string path, TextureFilterEnum filter = TextureFilterEnum.Nearest) => {
						_background.GetNode<TextureRect>("TextureRect").Texture = LoadImageFile(path, filter);
					}));
			currentWorld.Set("setTitle",
				new DelegateWrapper(CurrentEngine,
					(string title) => SetRichText(_game.GetNode<RichTextLabel>("%Title"), title)));

			currentWorld.Set("addLeftButton",
				new DelegateWrapper(CurrentEngine, AddLeftButton));
			currentWorld.Set("removeLeftButton",
				new DelegateWrapper(CurrentEngine, RemoveLeftButton));
			currentWorld.Set("setLeftButtons",
				new DelegateWrapper(CurrentEngine, SetLeftButtons));
			currentWorld.Set("addRightButton",
				new DelegateWrapper(CurrentEngine, AddRightButton));
			currentWorld.Set("removeRightButton",
				new DelegateWrapper(CurrentEngine, RemoveRightButton));
			currentWorld.Set("setm<RightButtons",
				new DelegateWrapper(CurrentEngine, SetRightButtons));

			currentWorld.Set("setLeftText",
				new DelegateWrapper(CurrentEngine,
					(string text) => {
						SetRichText(_leftText, text);
						_leftText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
					}));
			currentWorld.Set("setCenterText",
				new DelegateWrapper(CurrentEngine,
					(string text) => {
						SetRichText(_centerText, text);
						_centerText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
					}));
			currentWorld.Set("setRightText",
				new DelegateWrapper(CurrentEngine,
					(string text) => {
						SetRichText(_rightText, text);
						_rightText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
					}));
			currentWorld.Set("addLeftText",
				new DelegateWrapper(CurrentEngine,
					(string text) => {
						AddRichText(_leftText, text);
						_leftText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
					}));
			currentWorld.Set("addCenterText",
				new DelegateWrapper(CurrentEngine,
					(string text) => {
						AddRichText(_centerText, text);
						_centerText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
					}));
			currentWorld.Set("addRightText",
				new DelegateWrapper(CurrentEngine,
					(string text) => {
						AddRichText(_rightText, text);
						_rightText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
					}));
			currentWorld.Set("setLeftStretchRatio",
				new DelegateWrapper(CurrentEngine, (float ratio) => _leftText.SizeFlagsStretchRatio = ratio));
			currentWorld.Set("setCenterStretchRatio",
				new DelegateWrapper(CurrentEngine, (float ratio) => _centerText.SizeFlagsStretchRatio = ratio));
			currentWorld.Set("setRightStretchRatio",
				new DelegateWrapper(CurrentEngine, (float ratio) => _rightText.SizeFlagsStretchRatio = ratio));

			currentWorld.Set("getSaveValue",
				new DelegateWrapper(CurrentEngine,
					(string section, string key, JsValue defaultValue = null) => {
						var value = GetSaveValue(section, key).VariantToJsValue(CurrentEngine);
						return value == JsValue.Undefined ? defaultValue ?? JsValue.Undefined : value;
					}));
			currentWorld.Set("setSaveValue",
				new DelegateWrapper(CurrentEngine, SetSaveValue));
			currentWorld.Set("getGlobalSaveValue",
				new DelegateWrapper(CurrentEngine,
					(string section, string key, JsValue defaultValue = null) => {
						using var defaultGodotObject = new GodotObject();
						var variant = Utils.GlobalConfig.GetValue(section, key, defaultGodotObject);
						if (variant.Obj != defaultGodotObject) defaultGodotObject.Free();
						var value = variant.VariantToJsValue(CurrentEngine);
						return value == JsValue.Undefined ? defaultValue ?? JsValue.Undefined : value;
					}));
			currentWorld.Set("setGlobalSaveValue",
				new DelegateWrapper(CurrentEngine,
					(string section, string key, JsValue value) => {
						Utils.GlobalConfig.SetValue(section, key, value.JsValueToVariant(CurrentEngine));
						Utils.GlobalConfig.SaveEncryptedPass($"{Utils.SavesPath}/global.save", "global");
					}));

			currentWorld.Set("setCommandPlaceholderText",
				new DelegateWrapper(CurrentEngine, (string text) => _commandEdit.PlaceholderText = text));

			currentWorld.Set("exit",
				new DelegateWrapper(CurrentEngine, ExitWorld));
			CurrentEngine.SetValue("World", currentWorld);
		} catch (Exception e) {
			Log(e.ToString());
			ExitWorld(1);
		}
	}

	private void EmitEvent(WorldEventType name, params JsValue[] values) {
		try {
			_currentWorldEvent?["emit"].Call(thisObj: _currentWorldEvent, [name.ToString("G").ToSnakeCase(), ..values]);
		} catch (JavaScriptException e) {
			Log($"{e.Error}\n{StackTraceParser.ReTrace(Utils.SourceMapCollection, e.JavaScriptStackTrace ?? string.Empty)}");
			ExitWorld(1);
		}
	}

	private int[] SetLeftButtons(string[] names) {
		if (_leftButtonList.GetChildren().Select(node => ((Button)node).Text).SequenceEqual(names)) {
			return default;
		}

		foreach (var node in _leftButtonList.GetChildren()) {
			_leftButtonList.RemoveChild(node);
			node.QueueFree();
		}

		return names.Select(AddLeftButton).ToArray();
	}

	private int AddLeftButton(string str) {
		var button = new Button();
		_leftButtonList.AddChild(button);
		button.MouseFilter = MouseFilterEnum.Pass;
		button.Text = str;
		button.Pressed += () =>
			EmitEvent(WorldEventType.LeftButtonClick, str, button.GetIndex());
		return button.GetIndex();
	}

	private void RemoveLeftButton(int index) {
		if (index >= 0 ? index > _leftButtonList.GetChildCount() - 1 : index < -_leftButtonList.GetChildCount()) return;
		var node = _leftButtonList.GetChild(index);
		_leftButtonList.RemoveChild(node);
		node.QueueFree();
	}

	private int[] SetRightButtons(string[] names) {
		if (_rightButtonList.GetChildren().Select(node => ((Button)node).Text).SequenceEqual(names)) {
			return default;
		}

		foreach (var node in _rightButtonList.GetChildren()) {
			_rightButtonList.RemoveChild(node);
			node.QueueFree();
		}

		return names.Select(AddRightButton).ToArray();
	}

	private int AddRightButton(string str) {
		var button = new Button();
		_rightButtonList.AddChild(button);
		button.MouseFilter = MouseFilterEnum.Pass;
		button.Text = str;
		button.Pressed += () =>
			EmitEvent(WorldEventType.RightButtonClick, str, button.GetIndex());
		return button.GetIndex();
	}

	private void RemoveRightButton(int index) {
		if (index >= 0 ? index > _rightButtonList.GetChildCount() - 1 : index < -_rightButtonList.GetChildCount()) return;
		var node = _rightButtonList.GetChild(index);
		_rightButtonList.RemoveChild(node);
		node.QueueFree();
	}

	private int SetTimeout(JsValue callback, int delay, params JsValue[] values) {
		return AddTimer(false, callback, delay, values);
	}

	private int SetInterval(JsValue callback, int delay, params JsValue[] values) {
		return AddTimer(true, callback, delay, values);
	}

	private int AddTimer(bool autoReset, JsValue callback, int delay, params JsValue[] values) {
		if (delay <= 0) {
			callback.Call(thisObj: JsValue.Undefined, values ?? Array.Empty<JsValue>());
			CurrentEngine.Advanced.ProcessTasks();
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

			Callable.From(() => {
				try {
					callback.Call(thisObj: JsValue.Undefined, values ?? []);
					CurrentEngine.Advanced.ProcessTasks();
				} catch (JavaScriptException e) {
					Log(
						$"{e.Error}\n{StackTraceParser.ReTrace(Utils.SourceMapCollection, e.JavaScriptStackTrace ?? string.Empty)}");
					ExitWorld(1);
				} catch (ExecutionCanceledException) { }
			}).CallDeferred();
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

		Utils.Timers?.Clear();
		Utils.Tcs?.Cancel();
		Utils.Tcs = null;
		Utils.SourceMapCollection = null;

		foreach (var node in Utils.Tree.GetNodesInGroup("Audio").OfType<AudioPlayer>()) {
			node.Dispose();
		}

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

		_background.GetNode<ColorRect>("ColorRect").Color = Color.Color8(74, 74, 74);
		_background.GetNode<TextureRect>("TextureRect").Texture = null;
	}

	private void ExitWorld(int exitCode = 0) {
		Log("退出世界:", exitCode, CurrentWorldInfo.JsonString);
		EmitEvent(WorldEventType.Exit, exitCode);
		CurrentEngine.Dispose();
		CurrentEngine = null;
		_currentWorldEvent = null;
		_currentWorld = null;
		CurrentWorldInfo = null;

		ClearCache();

		_home.Visible = true;
		_game.Visible = false;
	}

	static private CanvasTexture LoadImageFile(string path, TextureFilterEnum filter = TextureFilterEnum.Linear) {
		return LoadImageFile(CurrentWorldInfo, path, filter);
	}

	static private CanvasTexture LoadImageFile(
		WorldInfo worldInfo,
		string path,
		TextureFilterEnum filter = TextureFilterEnum.Linear) {
		var filePath = worldInfo.GlobalPath.PathJoin(path).SimplifyPath();
		if (!FileAccess.FileExists(filePath)) return null;
		ImageTexture imageTexture = null;
		if (ResourceLoader.Exists(filePath)) {
			var canvasTexture = GD.Load<CanvasTexture>(filePath);
			if (canvasTexture.TextureFilter == filter) {
				return GD.Load<CanvasTexture>(filePath);
			}

			imageTexture = (ImageTexture)canvasTexture.DiffuseTexture;
		}

		var texture = new CanvasTexture();
		texture.TextureFilter = filter;
		texture.TakeOverPath(filePath);
		Utils.TextureCache.Add(texture);
		if (imageTexture == null) {
			var data = FileAccess.GetFileAsBytes(filePath);
			using var img = new Image();
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

			imageTexture = ImageTexture.CreateFromImage(img);
		}

		texture.DiffuseTexture = imageTexture;

		return texture;
	}

	static private Variant GetSaveValue(string section, string key) {
		using var defaultValue = new GodotObject();
		var value = CurrentWorldInfo.Config.GetValue(section, key, defaultValue);
		if (value.Obj != defaultValue) defaultValue.Free();
		return value;
	}

	static private void SetSaveValue(string section, string key, JsValue value) {
		CurrentWorldInfo.Config.SetValue(section, key, value.JsValueToVariant(CurrentEngine));
		CurrentWorldInfo.Config.SaveEncryptedPass($"{Utils.SavesPath}/{CurrentWorldInfo.Author}:{CurrentWorldInfo.Name}.save",
			$"{CurrentWorldInfo.Author}:{CurrentWorldInfo.Name}");
	}

	static private void SetRichText(RichTextLabel label, string text) {
		LoadRichTextImg(ref text);
		label.Text = text;
	}

	static private void AddRichText(RichTextLabel label, string text) {
		LoadRichTextImg(ref text);
		label.AppendText(text);
	}

	static private void LoadRichTextImg(ref string text) {
		if (string.IsNullOrEmpty(text)) return;
		foreach (Match match in Utils.ImgPathRegex().Matches(text)) {
			var path = match.Groups["path"].Value;
			var oldText = text.Substring(match.Index, match.Length);
			var filter = Utils.ParseExpressionsForValues(oldText);
			CanvasTexture texture;

			if (!string.IsNullOrEmpty(filter)) {
				texture = filter switch {
					"linear" => LoadImageFile(path),
					"nearest" => LoadImageFile(path, TextureFilterEnum.Nearest),
					_ => LoadImageFile(path)
				};
			} else {
				texture = LoadImageFile(path);
			}

			text = text.Replace(oldText, oldText.Replace(path, texture.ResourcePath));
		}
	}

	public static void Log(params object[] objects) { Log(string.Join(" ", objects.Select(o => o.ToString()))); }

	public static void Log(params string[] strings) { Log(strings.Join(" ")); }

	public static void Log(string str) {
		GD.Print(
			$"[{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}]{(CurrentWorldInfo == null ? " " : $" [{CurrentWorldInfo.Name}] ")}{str}");
	}
}