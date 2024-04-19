using System;
using System.Linq;
using Godot;
using Puerts;
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

namespace 创世记;

public partial class World : Control {
	[Export] public ColorRect BackgroundColor { get; set; }
	[Export] public TextureRect BackgroundTexture { get; set; }
	[Export] public RichTextLabel Title { get; set; }
	[Export] public Control LeftButtonList { get; set; }
	[Export] public Control RightButtonList { get; set; }
	[Export] public RichTextLabel LeftText { get; set; }
	[Export] public RichTextLabel CenterText { get; set; }
	[Export] public RichTextLabel RightText { get; set; }
	[Export] public LineEdit CommandEdit { get; set; }
	[Export] public Label Toast;

	private StyleBoxFlat _titleStyle;
	private StyleBoxFlat _leftTextStyle;
	private StyleBoxFlat _centerTextStyle;
	private StyleBoxFlat _rightTextStyle;
	private Tween? _toastTween;
	private JsEnv? _jsEnv;
	public JSObject? JsEvent { get; set; }
	public Action<EventType, object?[]?>? Emit { get; set; }
	public static World? Instance { get; private set; }

	public static void ThrowException(string message) {
		if (Instance?._jsEnv == null) return;
		PuertsDLL.ThrowException(Instance._jsEnv.Isolate, message);
	}

	public override void _Ready() {
		Instance = this;
		_titleStyle = (StyleBoxFlat)Title.GetThemeStylebox("normal");
		_leftTextStyle = (StyleBoxFlat)LeftText.GetParent().GetParent<Panel>().GetThemeStylebox("panel");
		_centerTextStyle = (StyleBoxFlat)CenterText.GetParent().GetParent<Panel>().GetThemeStylebox("panel");
		_rightTextStyle = (StyleBoxFlat)RightText.GetParent().GetParent<Panel>().GetThemeStylebox("panel");

		LeftText.MetaClicked += meta => OnMetaClickedEventHandler(meta, TextType.LeftText);
		CenterText.MetaClicked += meta => OnMetaClickedEventHandler(meta, TextType.CenterText);
		RightText.MetaClicked += meta => OnMetaClickedEventHandler(meta, TextType.RightText);

		CommandEdit.TextSubmitted += text => {
			CommandEdit.Text = "";
			EventEmit(EventType.Command, string.IsNullOrEmpty(text) ? null : text);
		};

		LeftButtonList.Resized += async () => {
			if (LeftButtonList.GetChildCount() <= 0) return;
			await ToSignal(Utils.Tree, SceneTree.SignalName.ProcessFrame);
			LeftButtonList.GetNode<SmoothScroll>("../..").ScrollToLeft(0);
		};
		LeftButtonList.VisibilityChanged += async () => {
			if (LeftButtonList.GetChildCount() <= 0) return;
			await ToSignal(Utils.Tree, SceneTree.SignalName.ProcessFrame);
			LeftButtonList.GetNode<SmoothScroll>("../..").ScrollToLeft(0);
		};

		RightButtonList.Resized += async () => {
			if (RightButtonList.GetChildCount() <= 0) return;
			await ToSignal(Utils.Tree, SceneTree.SignalName.ProcessFrame);
			RightButtonList.GetNode<SmoothScroll>("../..").ScrollToRight(0);
		};
		RightButtonList.VisibilityChanged += async () => {
			if (RightButtonList.GetChildCount() <= 0) return;
			await ToSignal(Utils.Tree, SceneTree.SignalName.ProcessFrame);
			RightButtonList.GetNode<SmoothScroll>("../..").ScrollToRight(0);
		};

		SetTitle($"{Main.CurrentWorldInfo!.Name}-{Main.CurrentWorldInfo.Version}");
		GetNode<Control>("%Main").Hide();
		GetNode<Button>("%Encrypt").Disabled = Main.CurrentWorldInfo.IsEncrypt;
		GetNode<Button>("%Log").Pressed += Log.LogWindow.ToggleVisible;
		GetNode<Button>("%Exit").Pressed += () => Utils.Tree.Root.PropagateNotification((int)NotificationWMGoBackRequest);
		GetNode<Button>("%Encrypt").Pressed += () => {
			Utils.ExportEncryptionWorldPck(Main.CurrentWorldInfo);
			Exit();
		};

		try {
			_jsEnv = new JsEnv(new WorldModuleLoader(this));
			_jsEnv.ExecuteModule(Main.CurrentWorldInfo.Main);
		} catch (Exception e) { 
			Log.Error(_jsEnv?.Eval<JSObject>($"World.getLastException(`{e.Message}`)")?.Get<string>("stack") ?? e.Message);
		}
	}

	public override void _Process(double delta) {
		if (Instance is null) return;
		_jsEnv?.Tick();
	}

	public override void _ExitTree() {
		base._ExitTree();
		_titleStyle.Dispose();
		_leftTextStyle.Dispose();
		_centerTextStyle.Dispose();
		_rightTextStyle.Dispose();
		_toastTween?.Dispose();
	}

	private void OnMetaClickedEventHandler(Variant meta, TextType type) {
		var str = meta.ToString();
		meta.Dispose();
		EventEmit(EventType.TextUrlClick, str, type);
	}

	public void EventEmit(EventType type, params object?[]? args) {
		if (Emit is null) {
			_jsEnv?.UsingAction<EventType, object?[]?>();
			Emit ??= JsEvent?.Get<Action<EventType, object?[]?>>("emit");
		}

		try {
			Emit?.Invoke(type, args);
		} catch (Exception e) {
			Log.Error(_jsEnv?.Eval<JSObject>($"World.getLastException(`{e.Message}`)")?.Get<string>("stack") ?? e.Message);
		}
	}

	public void Exit(int exitCode = 0) {
		if (Main.CurrentWorldInfo is null) return;
		EventEmit(EventType.Exit, exitCode);
		Log.Debug("退出世界:", exitCode.ToString(), Main.CurrentWorldInfo.JsonString);
		Instance = null;
		_jsEnv?.Dispose();
		_jsEnv = null;
		Main.CurrentWorldInfo = null;
		Main.ClearCache();
		SetProcess(false);
		SetPhysicsProcess(false);
		QueueFree();
	}

	public void ShowToast(string text) {
		Toast.Text = text;
		_toastTween?.Kill();
		_toastTween = CreateTween();
		_toastTween.TweenProperty(Toast, "modulate:a", 1, 0.5f);
		_toastTween.TweenProperty(Toast, "modulate:a", 0, 0.5f)
			.SetDelay(2.5);
	}

	public void SetBackgroundColor(string colorHex) {
		BackgroundColor.Color = Color.FromHtml(colorHex);
	}

	public void SetBackgroundTexture(string texturePath, FilterType filter = FilterType.Linear) {
		BackgroundTexture.Texture = Utils.LoadImageFile(texturePath, filter);
	}

	public void SetTitle(string title) => Utils.SetRichText(Title, title);

	public void SetStretchRatio(TextType type, float ratio) {
		if ((type & TextType.LeftText) > 0) {
			SetLeftStretchRatio(ratio);
		}

		if ((type & TextType.CenterText) > 0) {
			SetCenterStretchRatio(ratio);
		}

		if ((type & TextType.RightText) > 0) {
			SetRightStretchRatio(ratio);
		}
	}

	public void SetLeftStretchRatio(float ratio) => LeftText.GetParent().GetParent<Panel>().SizeFlagsStretchRatio = ratio;

	public void SetCenterStretchRatio(float ratio) => CenterText.GetParent().GetParent<Panel>().SizeFlagsStretchRatio = ratio;

	public void SetRightStretchRatio(float ratio) => RightText.GetParent().GetParent<Panel>().SizeFlagsStretchRatio = ratio;
	public void SetCommandPlaceholderText(string text) => CommandEdit.PlaceholderText = text;

	public int GetParagraphCount(TextType type) {
		var count = type switch {
			TextType.Title => Title.GetParagraphCount(),
			TextType.LeftText => LeftText.GetParagraphCount(),
			TextType.CenterText => CenterText.GetParagraphCount(),
			TextType.RightText => RightText.GetParagraphCount(),
			_ => -1
		};

		return count;
	}

	public bool RemoveParagraph(TextType type, int index) {
		var result = false;
		if (index < 0) {
			index = GetParagraphCount(type) + index;
		}

		result = type switch {
			TextType.Title => Title.RemoveParagraph(index),
			TextType.LeftText => LeftText.RemoveParagraph(index),
			TextType.CenterText => CenterText.RemoveParagraph(index),
			TextType.RightText => RightText.RemoveParagraph(index),
			_ => result
		};

		return result;
	}

	public void SetText(TextType textType, string text) {
		if ((textType & TextType.Title) > 0) {
			SetTitle(text);
		}

		if ((textType & TextType.LeftText) > 0) {
			SetLeftText(text);
		}

		if ((textType & TextType.CenterText) > 0) {
			SetCenterText(text);
		}

		if ((textType & TextType.RightText) > 0) {
			SetRightText(text);
		}
	}

	public void SetLeftText(string text) {
		Utils.SetRichText(LeftText, text);
	}

	public void SetCenterText(string text) {
		Utils.SetRichText(CenterText, text);
	}

	public void SetRightText(string text) {
		Utils.SetRichText(RightText, text);
	}

	public void AddText(TextType textType, string text) {
		if ((textType & TextType.Title) > 0) {
			AddTitle(text);
		}

		if ((textType & TextType.LeftText) > 0) {
			AddLeftText(text);
		}

		if ((textType & TextType.CenterText) > 0) {
			AddCenterText(text);
		}

		if ((textType & TextType.RightText) > 0) {
			AddRightText(text);
		}
	}

	public void AddTitle(string title) => Utils.AddRichText(Title, title);

	public void AddLeftText(string text) {
		Utils.AddRichText(LeftText, text);
	}

	public void AddCenterText(string text) {
		Utils.AddRichText(CenterText, text);
	}

	public void AddRightText(string text) {
		Utils.AddRichText(RightText, text);
	}

	public ulong[] SetLeftButtons(string names) {
		var namesArray = Json.ParseString(names).AsStringArray();
		ulong[] buttons;
		if (LeftButtonList.GetChildren().Select(node => ((Button)node).Text).SequenceEqual(namesArray)) {
			buttons = LeftButtonList.GetChildren().Select(node => ((Button)node).GetInstanceId()).ToArray();
			return buttons;
		}

		foreach (var node in LeftButtonList.GetChildren()) {
			LeftButtonList.RemoveChild(node);
			node.QueueFree();
		}

		buttons = namesArray.Select(AddLeftButton).ToArray();

		return buttons;
	}

	public ulong AddLeftButton(string str) {
		var button = new Button();
		LeftButtonList.AddChild(button);
		button.MouseFilter = MouseFilterEnum.Pass;
		button.Text = str;
		button.Pressed += () =>
			EventEmit(EventType.LeftButtonClick, str, button.GetIndex(), button.GetInstanceId());
		var id = button.GetInstanceId();
		return id;
	}

	public void RemoveLeftButtonByIndex(int index) {
		if (index >= 0 ? index > LeftButtonList.GetChildCount() - 1 : index < -LeftButtonList.GetChildCount()) return;
		var node = LeftButtonList.GetChild(index);
		node.QueueFree();
	}

	public ulong[] SetRightButtons(string names) {
		var namesArray = Json.ParseString(names).AsStringArray();
		ulong[] buttons;
		if (RightButtonList.GetChildren().Select(node => ((Button)node).Text).SequenceEqual(namesArray)) {
			buttons = RightButtonList.GetChildren().Select(node => ((Button)node).GetInstanceId()).ToArray();
			return buttons;
		}

		foreach (var node in RightButtonList.GetChildren()) {
			RightButtonList.RemoveChild(node);
			node.QueueFree();
		}

		buttons = namesArray.Select(AddRightButton).ToArray();

		return buttons;
	}

	public ulong AddRightButton(string str) {
		var button = new Button();
		RightButtonList.AddChild(button);
		button.MouseFilter = MouseFilterEnum.Pass;
		button.Text = str;
		button.Pressed += () =>
			EventEmit(EventType.RightButtonClick, str, button.GetIndex(), button.GetInstanceId());
		var id = button.GetInstanceId();
		return id;
	}

	public void RemoveRightButtonByIndex(int index) {
		if (index >= 0 ? index > RightButtonList.GetChildCount() - 1 : index < -RightButtonList.GetChildCount()) return;
		var node = RightButtonList.GetChild(index);
		node.QueueFree();
	}

	public void SetTextBackgroundColor(TextType type, string colorHex) {
		var color = Color.FromString(colorHex, Color.Color8(0, 0, 0, 96));
		SetTextBackgroundColor(type, color);
	}

	private void SetTextBackgroundColor(TextType type, Color color) {
		var modulate = color.A > 0 ? Colors.White : Colors.Transparent;
		if ((type & TextType.Title) > 0) {
			_titleStyle.BgColor = color;
			Title.Modulate = modulate;
		}

		if ((type & TextType.LeftText) > 0) {
			_leftTextStyle.BgColor = color;
			LeftText.GetParent().GetParent<Panel>().Modulate = modulate;
		}

		if ((type & TextType.CenterText) > 0) {
			_centerTextStyle.BgColor = color;
			CenterText.GetParent().GetParent<Panel>().Modulate = modulate;
		}

		if ((type & TextType.RightText) > 0) {
			_rightTextStyle.BgColor = color;
			RightText.GetParent().GetParent<Panel>().Modulate = modulate;
		}
	}

	public void SetTextFontColor(TextType type, string colorHex) {
		var color = Color.FromString(colorHex, Colors.White);
		SetTextFontColor(type, color);
	}

	private void SetTextFontColor(TextType type, Color color) {
		if ((type & TextType.Title) > 0) {
			Title.AddThemeColorOverride("default_color", color);
		}

		if ((type & TextType.LeftText) > 0) {
			LeftText.AddThemeColorOverride("default_color", color);
		}

		if ((type & TextType.CenterText) > 0) {
			CenterText.AddThemeColorOverride("default_color", color);
		}

		if ((type & TextType.RightText) > 0) {
			RightText.AddThemeColorOverride("default_color", color);
		}
	}

}