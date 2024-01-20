using Godot;

namespace 创世记;

public partial class World : Control {
	[GetNode("%LeftButtonList")] public Control LeftButtonList;
	[GetNode("%RightButtonList")] public Control RightButtonList;
	[GetNode("%LeftText")] public RichTextLabel LeftText;
	[GetNode("%CenterText")] public RichTextLabel CenterText;
	[GetNode("%RightText")] public RichTextLabel RightText;
	[GetNode("%CommandEdit")] public LineEdit CommandEdit;
}