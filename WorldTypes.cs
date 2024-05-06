using System;

namespace 创世记;

public enum FilterType {
	Linear,
	Nearest
}

public enum EventType {
	Ready,
	Exit,
	Command,
	LeftButtonClick,
	RightButtonClick,
	TextUrlClick
}

[Flags]
public enum TextType {
	Title = 1,
	LeftText = 2,
	CenterText = 4,
	RightText = 8,
	All = Title | LeftText | CenterText | RightText
}