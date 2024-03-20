using System;
using System.Linq;
using Godot;
using World;
// ReSharper disable MemberCanBePrivate.Global

namespace 创世记;

public partial class World : Control {
	[GetNode("%Title")] public RichTextLabel Title;
	[GetNode("%LeftButtonList")] public Control LeftButtonList;
	[GetNode("%RightButtonList")] public Control RightButtonList;
	[GetNode("%LeftText")] public RichTextLabel LeftText;
	[GetNode("%CenterText")] public RichTextLabel CenterText;
	[GetNode("%RightText")] public RichTextLabel RightText;
	[GetNode("%CommandEdit")] public LineEdit CommandEdit;
	[GetNode("%Toast")] public Label Toast;

	private StyleBoxFlat _titleStyle;
	private StyleBoxFlat _leftTextStyle;
	private StyleBoxFlat _centerTextStyle;
	private StyleBoxFlat _rightTextStyle;
	private Tween? _toastTween;


	public override void _Ready() {
		_titleStyle = (StyleBoxFlat)Title.GetThemeStylebox("normal");
		_leftTextStyle = (StyleBoxFlat)LeftText.GetParent().GetParent<Panel>().GetThemeStylebox("panel");
		_centerTextStyle = (StyleBoxFlat)CenterText.GetParent().GetParent<Panel>().GetThemeStylebox("panel");
		_rightTextStyle = (StyleBoxFlat)RightText.GetParent().GetParent<Panel>().GetThemeStylebox("panel");

		LeftText.MetaClicked += meta => Main.OnMetaClickedEventHandler(meta, 0);
		CenterText.MetaClicked += meta => Main.OnMetaClickedEventHandler(meta, 1);
		RightText.MetaClicked += meta => Main.OnMetaClickedEventHandler(meta, 2);

		LeftButtonList.Resized += async () => {
			if (LeftButtonList.GetChildCount() <= 0) return;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			LeftButtonList.GetNode<SmoothScroll>("../..").ScrollToLeft(0);
		};
		LeftButtonList.VisibilityChanged += async () => {
			if (LeftButtonList.GetChildCount() <= 0) return;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			LeftButtonList.GetNode<SmoothScroll>("../..").ScrollToLeft(0);
		};

		RightButtonList.Resized += async () => {
			if (RightButtonList.GetChildCount() <= 0) return;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			RightButtonList.GetNode<SmoothScroll>("../..").ScrollToRight(0);
		};
		RightButtonList.VisibilityChanged += async () => {
			if (RightButtonList.GetChildCount() <= 0) return;
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			RightButtonList.GetNode<SmoothScroll>("../..").ScrollToRight(0);
		};
	}

	public void ShowToast(string text) {
		this.SyncSend(_ => {
			Toast.Text = text;
			_toastTween?.Kill();
			_toastTween = CreateTween();
			_toastTween.TweenProperty(Toast, "modulate:a", 1, 0.5f);
			_toastTween.TweenProperty(Toast, "modulate:a", 0, 0.5f)
				.SetDelay(2.5);
		});
	}

	public void SetTitle(string title) => Utils.SetRichText(Title, title);
	public void SetLeftStretchRatio(float ratio) => this.SyncSend(_ => LeftText.GetParent().GetParent<Panel>().SizeFlagsStretchRatio = ratio);

	public void SetCenterStretchRatio(float ratio) => this.SyncSend(_ => CenterText.GetParent().GetParent<Panel>().SizeFlagsStretchRatio = ratio);

	public void SetRightStretchRatio(float ratio) => this.SyncSend(_ => RightText.GetParent().GetParent<Panel>().SizeFlagsStretchRatio = ratio);
	public void SetCommandPlaceholderText(string text) => this.SyncSend(_ => CommandEdit.PlaceholderText = text);

	public void SetLeftText(string text) {
		Utils.SetRichText(LeftText, text);
		this.SyncSend(_ => {
			LeftText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
		});
	}

	public void SetCenterText(string text) {
		Utils.SetRichText(CenterText, text);
		this.SyncSend(_ => {
			CenterText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
		});
	}

	public void SetRightText(string text) {
		Utils.SetRichText(RightText, text);
		this.SyncSend(_ => {
			RightText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
		});
	}

	public void AddLeftText(string text) {
		Utils.AddRichText(LeftText, text);
		this.SyncSend(_ => {
			LeftText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
		});
	}

	public void AddCenterText(string text) {
		Utils.AddRichText(CenterText, text);
		this.SyncSend(_ => {
			CenterText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
		});
	}

	public void AddRightText(string text) {
		Utils.AddRichText(RightText, text);
		this.SyncSend(_ => {
			RightText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
		});
	}

	public ulong[] SetLeftButtons(string[] names) {
		ulong[] buttons = [];
		this.SyncSend(_ => {
			if (LeftButtonList.GetChildren().Select(node => ((Button)node).Text).SequenceEqual(names)) {
				buttons = LeftButtonList.GetChildren().Select(node => ((Button)node).GetInstanceId()).ToArray();
				return;
			}

			foreach (var node in LeftButtonList.GetChildren()) {
				LeftButtonList.RemoveChild(node);
				node.QueueFree();
			}

			buttons = names.Select(AddLeftButton).ToArray();
		});

		return buttons;
	}

	public ulong AddLeftButton(string str) {
		ulong id = 0;
		this.SyncSend(_ => {
			var button = new Button();
			LeftButtonList.AddChild(button);
			button.MouseFilter = MouseFilterEnum.Pass;
			button.Text = str;
			button.Pressed += () =>
				Main.EmitEvent(EventType.LeftButtonClick, str, button.GetIndex(), button.GetInstanceId());
			id = button.GetInstanceId();
		});
		return id;
	}

	public void RemoveLeftButtonByIndex(int index) {
		this.SyncSend(_ => {
			if (index >= 0 ? index > LeftButtonList.GetChildCount() - 1 : index < -LeftButtonList.GetChildCount()) return;
			var node = LeftButtonList.GetChild(index);
			node.QueueFree();
		});
	}

	public ulong[] SetRightButtons(string[] names) {
		ulong[] buttons = [];
		this.SyncSend(_ => {
			if (RightButtonList.GetChildren().Select(node => ((Button)node).Text).SequenceEqual(names)) {
				buttons = RightButtonList.GetChildren().Select(node => ((Button)node).GetInstanceId()).ToArray();
				return;
			}

			foreach (var node in RightButtonList.GetChildren()) {
				RightButtonList.RemoveChild(node);
				node.QueueFree();
			}

			buttons = names.Select(AddRightButton).ToArray();
		});

		return buttons;
	}

	public ulong AddRightButton(string str) {
		ulong id = 0;
		this.SyncSend(_ => {
			var button = new Button();
			RightButtonList.AddChild(button);
			button.MouseFilter = MouseFilterEnum.Pass;
			button.Text = str;
			button.Pressed += () =>
				Main.EmitEvent(EventType.RightButtonClick, str, button.GetIndex(), button.GetInstanceId());
			id = button.GetInstanceId();
		});
		return id;
	}

	public void RemoveRightButtonByIndex(int index) {
		this.SyncSend(_ => {
			if (index >= 0 ? index > RightButtonList.GetChildCount() - 1 : index < -RightButtonList.GetChildCount()) return;
			var node = RightButtonList.GetChild(index);
			node.QueueFree();
		});
	}

	public static void RemoveButtonById(ulong id) {
		var button = InstanceFromId(id) as Button;
		button?.QueueFree();
	}

	public void SetTextBackgroundColor(TextType type, string colorHex) {
		var color = Color.FromString(colorHex, Color.Color8(0, 0, 0, 96));
		if (type != TextType.All) {
			SetTextBackgroundColor(type, color);
		} else {
			foreach (TextType t in Enum.GetValuesAsUnderlyingType<TextType>()) {
				SetTextBackgroundColor(t, color);
			}
		}
	}

	private void SetTextBackgroundColor(TextType type, Color color) {
		this.SyncSend(_ => {
			switch (type) {
				case TextType.Title:
					_titleStyle.BgColor = color;
					break;
				case TextType.LeftText:
					_leftTextStyle.BgColor = color;
					break;
				case TextType.CenterText:
					_centerTextStyle.BgColor = color;
					break;
				case TextType.RightText:
					_rightTextStyle.BgColor = color;
					break;
				case TextType.All:
				default: return;
			}
		});
	}

	public void SetTextFontColor(TextType type, string colorHex) {
		var color = Color.FromString(colorHex, Colors.White);
		if (type != TextType.All) {
			SetTextFontColor(type, color);
		} else {
			foreach (TextType t in Enum.GetValuesAsUnderlyingType<TextType>()) {
				SetTextFontColor(t, color);
			}
		}
	}

	private void SetTextFontColor(TextType type, Color color) {
		this.SyncSend(_ => {
			switch (type) {
				case TextType.Title:
					Title.AddThemeColorOverride("default_color", color);
					break;
				case TextType.LeftText:
					LeftText.AddThemeColorOverride("default_color", color);
					break;
				case TextType.CenterText:
					CenterText.AddThemeColorOverride("default_color", color);
					break;
				case TextType.RightText:
					RightText.AddThemeColorOverride("default_color", color);
					break;
				case TextType.All:
				default: return;
			}
		});
	}
}