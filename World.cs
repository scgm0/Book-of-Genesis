using System.Linq;
using Godot;

namespace 创世记;

public partial class World : Control {
	[GetNode("%Title")] public RichTextLabel Title;
	[GetNode("%LeftButtonList")] public Control LeftButtonList;
	[GetNode("%RightButtonList")] public Control RightButtonList;
	[GetNode("%LeftText")] public RichTextLabel LeftText;
	[GetNode("%CenterText")] public RichTextLabel CenterText;
	[GetNode("%RightText")] public RichTextLabel RightText;
	[GetNode("%CommandEdit")] public LineEdit CommandEdit;

	public void SetTitle(string title) => Main.SetRichText(Title, title);
	public void SetLeftStretchRatio(float ratio) => LeftText.GetParent().GetParent<Panel>().SizeFlagsStretchRatio = ratio;
	public void SetCenterStretchRatio(float ratio) => CenterText.GetParent().GetParent<Panel>().SizeFlagsStretchRatio = ratio;
	public void SetRightStretchRatio(float ratio) => RightText.GetParent().GetParent<Panel>().SizeFlagsStretchRatio = ratio;
	public void SetCommandPlaceholderText(string text) => CommandEdit.PlaceholderText = text;

	public void SetLeftText(string text) {
		Main.SetRichText(LeftText, text);
		LeftText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
	}

	public void SetCenterText(string text) {
		Main.SetRichText(CenterText, text);
		CenterText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
	}

	public void SetRightText(string text) {
		Main.SetRichText(RightText, text);
		RightText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
	}

	public void AddLeftText(string text) {
		Main.AddRichText(LeftText, text);
		LeftText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
	}

	public void AddCenterText(string text) {
		Main.AddRichText(CenterText, text);
		CenterText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
	}

	public void AddRightText(string text) {
		Main.AddRichText(RightText, text);
		RightText.GetParent().GetParent<Panel>().Visible = !string.IsNullOrEmpty(text);
	}

	public ulong[] SetLeftButtons(string[] names) {
		if (LeftButtonList.GetChildren().Select(node => ((Button)node).Text).SequenceEqual(names)) {
			return LeftButtonList.GetChildren().Select(node => ((Button)node).GetInstanceId()).ToArray();
		}

		foreach (var node in LeftButtonList.GetChildren()) {
			LeftButtonList.RemoveChild(node);
			node.QueueFree();
		}

		return names.Select(AddLeftButton).ToArray();
	}

	public ulong AddLeftButton(string str) {
		var button = new Button();
		LeftButtonList.AddChild(button);
		button.MouseFilter = MouseFilterEnum.Pass;
		button.Text = str;
		button.Pressed += () =>
			Main.EmitEvent(WorldEventType.LeftButtonClick, str, button.GetIndex(), button.GetInstanceId());
		return button.GetInstanceId();
	}

	public void RemoveLeftButtonByIndex(int index) {
		GD.Print(LeftButtonList.GetChild(index));
		if (index >= 0 ? index > LeftButtonList.GetChildCount() - 1 : index < -LeftButtonList.GetChildCount()) return;
		var node = LeftButtonList.GetChild(index);
		node.QueueFree();
	}

	public ulong[] SetRightButtons(string[] names) {
		if (RightButtonList.GetChildren().Select(node => ((Button)node).Text).SequenceEqual(names)) {
			return RightButtonList.GetChildren().Select(node => ((Button)node).GetInstanceId()).ToArray();
		}

		foreach (var node in RightButtonList.GetChildren()) {
			RightButtonList.RemoveChild(node);
			node.QueueFree();
		}

		return names.Select(AddRightButton).ToArray();
	}

	public ulong AddRightButton(string str) {
		var button = new Button();
		RightButtonList.AddChild(button);
		button.MouseFilter = MouseFilterEnum.Pass;
		button.Text = str;
		button.Pressed += () =>
			Main.EmitEvent(WorldEventType.RightButtonClick, str, button.GetIndex(), button.GetInstanceId());
		return button.GetInstanceId();
	}

	public void RemoveRightButtonByIndex(int index) {
		if (index >= 0 ? index > RightButtonList.GetChildCount() - 1 : index < -RightButtonList.GetChildCount()) return;
		var node = RightButtonList.GetChild(index);
		node.QueueFree();
	}

	public static void RemoveButtonById(ulong id) {
		var button = InstanceFromId(id) as Button;
		button?.QueueFree();
	}

}