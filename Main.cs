using System;
using System.Collections.Generic;
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
using Timer = System.Timers.Timer;

namespace 创世记;

public sealed partial class Main : Control {
	[GetNode("%ChooseWorldButton")] private Button _chooseWorldButton;
	[GetNode("%Home")] private Control _home;
	[GetNode("%Game")] private Control _game;
	[GetNode("%ChooseWorld")] private Control _chooseWorld;
	[GetNode("%TemplateWorldButton")] private Button _templateWorldButton;
	[GetNode("%Background")] private Control _background;
	[GetNode("%Game/%LeftButtonList")] private Control _leftButtonList;
	[GetNode("%Game/%RightButtonList")] private Control _rightButtonList;
	[GetNode("%Game/%LeftText")] private RichTextLabel _leftText;
	[GetNode("%Game/%CenterText")] private RichTextLabel _centerText;
	[GetNode("%Game/%RightText")] private RichTextLabel _rightText;
	[GetNode("%Game/%CommandEdit")] private LineEdit _commandEdit;

	[Export] private PackedScene _gameScene;
	[Export] private PackedScene _worldItem;

	private readonly Dictionary<int, Timer> _timers = new();
	private readonly ConfigFile _globalConfig = new();

	private JsonParser _jsonParser;

	public override void _Ready() {
		if (Utils.IsAndroid) {
			OS.RequestPermissions();
		}

		GetTree().AutoAcceptQuit = false;

		_home.GetNode<LinkButton>("ModsPathHint").Text =
			$"世界存放位置：{ProjectSettings.GlobalizePath(Utils.UserModsPath.SimplifyPath())}";
		_home.GetNode<LinkButton>("ModsPathHint").Uri =
			$"{ProjectSettings.GlobalizePath(Utils.UserModsPath.SimplifyPath())}";
		_chooseWorldButton.Pressed += ChooseWorld;
		_templateWorldButton.Pressed += () => {
			DirAccess.MakeDirRecursiveAbsolute($"{Utils.UserModsPath}/模版世界");
			using var dir = DirAccess.Open("res://TemplateWorld/");
			if (dir == null) return;
			dir.ListDirBegin();
			var fileName = dir.GetNext();
			while (fileName is not "" and not "." and not "..") {
				if (!dir.CurrentIsDir() && fileName.GetExtension() != "import") {
					dir.Copy($"res://TemplateWorld/{fileName}", $"{Utils.UserModsPath}/模版世界/{fileName}");
				}

				fileName = dir.GetNext();
			}
		};
		GetNode<Button>("Window/ChooseWorld/Back").Pressed +=
			() => GetTree().Root.PropagateNotification((int)NotificationWMGoBackRequest);
		_game.GetNode<Button>("%Exit").Pressed += () => GetTree().Root.PropagateNotification((int)NotificationWMGoBackRequest);
		_game.GetNode<Button>("%Encrypt").Pressed += () => {
			Utils.ExportEncryptionModPck(CurrentModInfo);
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
			_leftText.GetNode<ScrollContainer>("../..").CallDeferred("scroll_to_left", 0);
		};
		_leftText.VisibilityChanged += async () => {
			if (_leftText.GetChildCount() <= 0) return;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			_leftText.GetNode<ScrollContainer>("../..").CallDeferred("scroll_to_left", 0);
		};
		_rightButtonList.Resized += async () => {
			if (_rightButtonList.GetChildCount() <= 0) return;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			_rightButtonList.GetNode<ScrollContainer>("../..").CallDeferred("scroll_to_right", 0);
		};
		_rightButtonList.VisibilityChanged += async () => {
			if (_rightButtonList.GetChildCount() <= 0) return;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			_rightButtonList.GetNode<ScrollContainer>("../..").CallDeferred("scroll_to_right", 0);
		};

		Utils.TsTransform.Prepare();
	}

	public override void _Notification(int what) {
		if (what == NotificationWMCloseRequest) {
			if (CurrentEngine != null) {
				ExitWorld();
			}

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
		LoadModInfos();
		_home.Visible = false;
		_chooseWorld.Visible = true;
		var list = GetNode<VBoxContainer>("Window/ChooseWorld/SmoothScrollContainer/MarginContainer/ChooseWorldList");
		foreach (var child in list.GetChildren()) {
			child.QueueFree();
		}

		foreach (var modInfo in ModInfos) {
			Log(modInfo.Key, modInfo.Value.JsonString);
			var worldItem = _worldItem.Instantiate();
			worldItem.GetNode<Label>("%Name").Text = $"{modInfo.Value.Name}-{modInfo.Value.Version}";
			worldItem.GetNode<Label>("%Description").Text = $"{modInfo.Value.Author}\n{modInfo.Value.Description}";
			worldItem.GetNode<TextureRect>("%Encrypt").Visible = modInfo.Value.IsEncrypt;
			var iconPath = (modInfo.Value.IsUser ? Utils.UserModsPath : Utils.ResModsPath).PathJoin(modInfo.Value.Path)
				.PathJoin(modInfo.Value.Icon);
			if (FileAccess.FileExists(iconPath)) {
				worldItem.GetNode<TextureRect>("%Icon").Texture = ImageTexture.CreateFromImage(Image.LoadFromFile(iconPath));
			}

			worldItem.GetNode<Button>("%Run").Pressed += () => {
				CurrentModInfo = modInfo.Value;
				Log("Run:", CurrentModInfo.JsonString);
				_chooseWorld.Visible = false;
				_background.Modulate = Color.FromHtml("#ffffff00");
				_game.Visible = true;
				_game.GetNode<RichTextLabel>("%Title").Text = $"{modInfo.Value.Name}-{modInfo.Value.Version}";
				_game.GetNode<Button>("%Encrypt").Disabled = modInfo.Value.IsEncrypt;
				modInfo.Value.Config.LoadEncryptedPass($"{Utils.SavesPath}/{modInfo.Value.Author}:{modInfo.Value.Name}.save",
					$"{modInfo.Value.Author}:{modInfo.Value.Name}");
				modInfo.Value.Config.SaveEncryptedPass($"{Utils.SavesPath}/{modInfo.Value.Author}:{modInfo.Value.Name}.save",
					$"{modInfo.Value.Author}:{modInfo.Value.Name}");
				_globalConfig.LoadEncryptedPass($"{Utils.SavesPath}/global.save", "global");
				_globalConfig.SaveEncryptedPass($"{Utils.SavesPath}/global.save", "global");
				RunWorld();
			};
			list.AddChild(worldItem);
		}
	}

	private void RunWorld() {
		try {
			_game.GetNode<Control>("Main").Visible = false;
			// RestoreDefaultSettings();
			InitEngine();
			CurrentEngine.Modules.Import(CurrentModInfo.Main);
			var tween = GetTree().CreateTween();
			// tween.SetTrans(Tween.TransitionType.Linear);
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

	private void InitEngine() {
		try {
			Utils.SourceMapCollection = new SourceMapCollection();
			_tcs = new CancellationTokenSource();
			CurrentEngine = new Engine(options => {
				options.CancellationToken(new CancellationToken(true));
				options.EnableModules(new CustomModuleLoader(CurrentModInfo));
			});
			_jsonParser = new JsonParser(CurrentEngine);
			var constraint = CurrentEngine.Constraints.Find<CancellationConstraint>();
			constraint?.Reset(_tcs.Token);
			CurrentEngine.SetValue("print", new Action<string[]>(Log))
				.SetValue("setTimeout", SetTimeout)
				.SetValue("setInterval", SetInterval)
				.SetValue("clearTimeout",
					(int id) => {
						if (!_timers.TryGetValue(id, out var value)) return;
						value?.Stop();
						_timers.Remove(id);
						value?.Dispose();
					})
				.SetValue("clearInterval",
					(int id) => {
						if (!_timers.TryGetValue(id, out var value)) return;
						value?.Stop();
						_timers.Remove(id);
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
						() => _jsonParser.Parse(CurrentModInfo.JsonString)),
					null));
			currentWorld.Set("setBackgroundColor",
				new DelegateWrapper(CurrentEngine,
					(string color) => {
						_background.GetNode<ColorRect>("ColorRect").Color =
							Color.FromString(color, Color.Color8(74, 74, 74)) with { A = 1 };
					}));
			currentWorld.Set("setBackgroundTexture",
				new DelegateWrapper(CurrentEngine,
					(string path) => { _background.GetNode<TextureRect>("TextureRect").Texture = LoadImageFile(path); }));
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
						var variant = _globalConfig.GetValue(section, key, defaultGodotObject);
						if (variant.Obj != defaultGodotObject) defaultGodotObject.Free();
						var value = variant.VariantToJsValue(CurrentEngine);
						return value == JsValue.Undefined ? defaultValue ?? JsValue.Undefined : value;
					}));
			currentWorld.Set("setGlobalSaveValue",
				new DelegateWrapper(CurrentEngine,
					(string section, string key, JsValue value) => {
						_globalConfig.SetValue(section, key, value.JsValueToVariant(CurrentEngine));
						_globalConfig.SaveEncryptedPass($"{Utils.SavesPath}/global.save", "global");
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
		_timers[id] = timer;
		timer.AutoReset = autoReset;
		timer.Elapsed += (_, _) => {
			if (!_timers.ContainsKey(id)) {
				timer.Dispose();
				return;
			}

			Callable.From(() => {
				callback.Call(thisObj: JsValue.Undefined, values ?? []);
				CurrentEngine.Advanced.ProcessTasks();
			}).CallDeferred();
			if (autoReset) return;
			_timers.Remove(id);
			timer.Dispose();
		};
		timer.Enabled = true;
		return id;
	}

	private void RestoreDefaultSettings() {
		foreach (var keyValuePair in _timers) {
			keyValuePair.Value.Stop();
			keyValuePair.Value.Dispose();
		}

		_timers.Clear();
		_tcs.Cancel();

		foreach (var node in Utils.Tree.GetNodesInGroup("Audio")) {
			(node as AudioPlayer)?.Dispose();
		}

		SetLeftButtons([]);
		SetRightButtons([]);

		_leftText.Clear();
		_centerText.Clear();
		_rightText.Clear();
		SetRichText(_leftText, null);
		SetRichText(_centerText, null);
		SetRichText(_rightText, null);
		_leftText.SizeFlagsStretchRatio = 1;
		_centerText.SizeFlagsStretchRatio = 1;
		_rightText.SizeFlagsStretchRatio = 1;
		_leftText.GetParent().GetParent<Panel>().Visible = false;
		_centerText.GetParent().GetParent<Panel>().Visible = false;
		_rightText.GetParent().GetParent<Panel>().Visible = false;

		_commandEdit.Text = "";
		_commandEdit.PlaceholderText = "";

		_background.GetNode<ColorRect>("ColorRect").Color = Color.Color8(74, 74, 74);
		_background.GetNode<TextureRect>("TextureRect").Texture = null;
	}

	private void ExitWorld(int exitCode = 0) {
		Log("Exit:", exitCode, CurrentModInfo.JsonString);
		EmitEvent(WorldEventType.Exit, exitCode);
		CurrentEngine.Dispose();
		CurrentEngine = null;
		_currentWorldEvent = null;
		_currentWorld = null;
		CurrentModInfo = null;

		RestoreDefaultSettings();

		_home.Visible = true;
		_game.Visible = false;
	}

	static private Texture2D LoadImageFile(string path) {
		path = (CurrentModInfo.IsUser ? Utils.UserModsPath : Utils.ResModsPath).PathJoin(CurrentModInfo.Path)
			.PathJoin(path).SimplifyPath();
		if (!FileAccess.FileExists(path)) return null;
		if (ResourceLoader.Exists(path)) {
			return GD.Load<Texture2D>(path);
		}

		using var img = Image.LoadFromFile(path);
		var texture = ImageTexture.CreateFromImage(img);
		texture.TakeOverPath(path);

		return texture;
	}

	static private Variant GetSaveValue(string section, string key) {
		using var defaultValue = new GodotObject();
		var value = CurrentModInfo.Config.GetValue(section, key, defaultValue);
		if (value.Obj != defaultValue) defaultValue.Free();
		return value;
	}

	static private void SetSaveValue(string section, string key, JsValue value) {
		CurrentModInfo.Config.SetValue(section, key, value.JsValueToVariant(CurrentEngine));
		CurrentModInfo.Config.SaveEncryptedPass($"{Utils.SavesPath}/{CurrentModInfo.Author}:{CurrentModInfo.Name}.save",
			$"{CurrentModInfo.Author}:{CurrentModInfo.Name}");
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
			text = text.Replace(oldText, oldText.Replace(path, LoadImageFile(path).ResourcePath));
		}
	}

	public static void Log(params object[] strings) { Log(string.Join(" ", strings.Select(o => o.ToString()))); }

	public static void Log(params string[] strings) { Log(strings.Join(" ")); }

	public static void Log(string str) {
		GD.Print(
			$"[{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}]{(CurrentModInfo == null ? " " : $" [{CurrentModInfo.Name}] ")}{str}");
	}

}