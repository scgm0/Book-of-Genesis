// ReSharper disable once CheckNamespace
namespace World;

public enum FilterType {
	Linear,
	Nearest
}

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