// ReSharper disable once CheckNamespace
namespace World;

public enum EventType {
	Ready,
	Tick,
	Exit,

	Command,
	LeftButtonClick,
	RightButtonClick,
	TextUrlClick
}

public enum TextType {
	All,
	Title,
	LeftText,
	CenterText,
	RightText
}