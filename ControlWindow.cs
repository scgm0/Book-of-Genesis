using System;
using Godot;
// ReSharper disable CheckNamespace

[Tool]
[GlobalClass]
#pragma warning disable CA1050
public partial class ControlWindow : PanelContainer {
#pragma warning restore CA1050
	[Flags]
	private enum ResizeDirection {
		Left = 1 << 1,
		Top = 1 << 2,
		Right = 1 << 3,
		Bottom = 1 << 4
	}

	private StyleBoxFlat _decorationsStyle = new() {
		BgColor = Color.Color8(56, 56, 56),
		CornerRadiusTopLeft = 3,
		CornerRadiusTopRight = 3,
		CornerRadiusBottomLeft = 3,
		CornerRadiusBottomRight = 3,
		ExpandMarginLeft = 8,
		ExpandMarginTop = 32,
		ExpandMarginRight = 8,
		ExpandMarginBottom = 8
	};

	private Panel _decorations;
	private MarginContainer _tabBar;
	private Label _title;
	private Vector2 _relativePos;
	private Vector2? _defaultSize;
	private bool _dragging;
	private bool _resizable;
	[Export] public string Title { get => _title.Text; set => _title.Text = value; }

	public ControlWindow() {
		var defaultTheme = ThemeDB.GetDefaultTheme();

		_decorations = new Panel {
			Name = "Decorations",
			ShowBehindParent = true
		};

		_decorations.AddThemeStyleboxOverride("panel", _decorationsStyle);

		{
			_tabBar = new MarginContainer {
				Name = "TabBar",
				AnchorRight = 1,
				OffsetTop = -_decorationsStyle.ExpandMarginTop,
				GrowHorizontal = GrowDirection.Begin,
				FocusMode = FocusModeEnum.Click
			};

			_tabBar.GuiInput += OnTabBarGuiInput;
			_tabBar.AddThemeConstantOverride("margin_right", 18);
			_tabBar.AddThemeConstantOverride("margin_left", 18);

			{
				_title = new Label {
					Name = "Title",
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis
				};

				{
					var closeButton = new TextureButton {
						Name = "CloseButton",
						StretchMode = TextureButton.StretchModeEnum.KeepAspectCentered,
						TextureNormal = defaultTheme.GetIcon("close", "Window"),
						TexturePressed = defaultTheme.GetIcon("close_pressed", "Window"),
						CustomMinimumSize = new Vector2(18, 18),
						AnchorLeft = 1,
						AnchorTop = 0.5f,
						AnchorRight = 1,
						AnchorBottom = 0.5f,
						OffsetTop = -9,
						OffsetRight = 18,
						OffsetBottom = 9
					};

					closeButton.Pressed += () => Notification((int)NotificationWMCloseRequest, true);
					_title.AddChild(closeButton);
				}

				_tabBar.AddChild(_title);
			}

			_decorations.AddChild(_tabBar);
		}

		var resizeMargin = new MarginContainer {
			Name = "ResizeMargin",
		};

		resizeMargin.AddThemeConstantOverride("margin_left", -8);
		resizeMargin.AddThemeConstantOverride("margin_top", -32);
		resizeMargin.AddThemeConstantOverride("margin_right", -8);
		resizeMargin.AddThemeConstantOverride("margin_bottom", -8);
		{
			var resizeLeft = new Control {
				Name = "ResizeLeft",
				CustomMinimumSize = new Vector2(8, 0),
				SizeFlagsHorizontal = SizeFlags.ShrinkBegin,
				MouseDefaultCursorShape = CursorShape.Hsize,
				FocusMode = FocusModeEnum.Click
			};
			var resizeTop = new Control {
				Name = "ResizeTop",
				CustomMinimumSize = new Vector2(0, 8),
				SizeFlagsVertical = SizeFlags.ShrinkBegin,
				MouseDefaultCursorShape = CursorShape.Vsize,
				FocusMode = FocusModeEnum.Click
			};
			var resizeRight = new Control {
				Name = "ResizeRight",
				CustomMinimumSize = new Vector2(8, 0),
				SizeFlagsHorizontal = SizeFlags.ShrinkEnd,
				MouseDefaultCursorShape = CursorShape.Hsize,
				FocusMode = FocusModeEnum.Click
			};
			var resizeBottom = new Control {
				Name = "ResizeBottom",
				CustomMinimumSize = new Vector2(0, 8),
				SizeFlagsVertical = SizeFlags.ShrinkEnd,
				MouseDefaultCursorShape = CursorShape.Vsize,
				FocusMode = FocusModeEnum.Click
			};
			var resizeLeftTop = new Control {
				Name = "ResizeLeftTop",
				CustomMinimumSize = new Vector2(8, 8),
				SizeFlagsHorizontal = SizeFlags.ShrinkBegin,
				SizeFlagsVertical = SizeFlags.ShrinkBegin,
				MouseDefaultCursorShape = CursorShape.Fdiagsize,
				FocusMode = FocusModeEnum.Click
			};
			var resizeRightTop = new Control {
				Name = "ResizeRightTop",
				CustomMinimumSize = new Vector2(8, 8),
				SizeFlagsHorizontal = SizeFlags.ShrinkEnd,
				SizeFlagsVertical = SizeFlags.ShrinkBegin,
				MouseDefaultCursorShape = CursorShape.Bdiagsize,
				FocusMode = FocusModeEnum.Click
			};
			var resizeRightBottom = new Control {
				Name = "ResizeRightBottom",
				CustomMinimumSize = new Vector2(8, 8),
				SizeFlagsHorizontal = SizeFlags.ShrinkEnd,
				SizeFlagsVertical = SizeFlags.ShrinkEnd,
				MouseDefaultCursorShape = CursorShape.Fdiagsize,
				FocusMode = FocusModeEnum.Click
			};
			var resizeLeftBottom = new Control {
				Name = "ResizeLeftBottom",
				CustomMinimumSize = new Vector2(8, 8),
				SizeFlagsHorizontal = SizeFlags.ShrinkBegin,
				SizeFlagsVertical = SizeFlags.ShrinkEnd,
				MouseDefaultCursorShape = CursorShape.Bdiagsize,
				FocusMode = FocusModeEnum.Click
			};

			resizeLeft.GuiInput += @event => OnResized(@event, ResizeDirection.Left);
			resizeTop.GuiInput += @event => OnResized(@event, ResizeDirection.Top);
			resizeRight.GuiInput += @event => OnResized(@event, ResizeDirection.Right);
			resizeBottom.GuiInput += @event => OnResized(@event, ResizeDirection.Bottom);
			resizeLeftTop.GuiInput += @event => OnResized(@event, ResizeDirection.Left | ResizeDirection.Top);
			resizeRightTop.GuiInput += @event => OnResized(@event, ResizeDirection.Right | ResizeDirection.Top);
			resizeRightBottom.GuiInput += @event => OnResized(@event, ResizeDirection.Right | ResizeDirection.Bottom);
			resizeLeftBottom.GuiInput += @event => OnResized(@event, ResizeDirection.Left | ResizeDirection.Bottom);

			resizeMargin.AddChild(resizeLeft);
			resizeMargin.AddChild(resizeTop);
			resizeMargin.AddChild(resizeRight);
			resizeMargin.AddChild(resizeBottom);
			resizeMargin.AddChild(resizeLeftTop);
			resizeMargin.AddChild(resizeRightTop);
			resizeMargin.AddChild(resizeRightBottom);
			resizeMargin.AddChild(resizeLeftBottom);
		}

		AddChild(_decorations, false, InternalMode.Front);
		AddChild(resizeMargin, false, InternalMode.Front);

		AddThemeStyleboxOverride("panel",
			new StyleBoxFlat {
				BgColor = Color.Color8(77, 77, 77)
			});

		AnchorLeft = 0.5f;
		AnchorTop = 0.5f;
		AnchorRight = 0.5f;
		AnchorBottom = 0.5f;
		GrowHorizontal = GrowDirection.Both;
		GrowVertical = GrowDirection.Both;
		ChildEnteredTree += node => {
			if (node is not Control control) return;
			control.SizeFlagsHorizontal = SizeFlags.Fill;
			control.SizeFlagsVertical = SizeFlags.Fill;
		};
		VisibilityChanged += () => {
			_defaultSize ??= Size;
			OffsetLeft = -_defaultSize.Value.X / 2;
			OffsetTop = -_defaultSize.Value.Y / 2;
			OffsetRight = _defaultSize.Value.X / 2;
			OffsetBottom = _defaultSize.Value.Y / 2;

			Position = Position with { Y = Position.Y + (GetTabBarHeight() - GetDecorationsWidth()) / 2 };
		};
	}

	public float GetTabBarHeight() => _decorationsStyle.ExpandMarginTop;
	public float GetDecorationsWidth() => _decorationsStyle.ExpandMarginRight;

	private void ResizeWindow(ResizeDirection resizeDirection, Vector2 relative) {
		if ((resizeDirection & ResizeDirection.Left) == ResizeDirection.Left) {
			var oldX = Size.X;
			Size = Size with { X = Size.X - relative.X };
			Position = Position with { X = Position.X + oldX - Size.X };
		}

		if ((resizeDirection & ResizeDirection.Top) == ResizeDirection.Top) {
			var oldY = Size.Y;
			Size = Size with { Y = Size.Y - relative.Y };
			Position = Position with { Y = Position.Y + oldY - Size.Y };
		}

		if ((resizeDirection & ResizeDirection.Right) == ResizeDirection.Right) {
			Size = Size with { X = Size.X + relative.X };
		}

		if ((resizeDirection & ResizeDirection.Bottom) == ResizeDirection.Bottom) {
			Size = Size with { Y = Size.Y + relative.Y };
		}
	}

	private void OnTabBarGuiInput(InputEvent @event) {
		switch (@event) {
			case InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true } mouseButton:
				_dragging = true;
				_tabBar.MouseDefaultCursorShape = CursorShape.Drag;
				_relativePos = mouseButton.Position;
				break;
			case InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false }:
				_dragging = false;
				_tabBar.MouseDefaultCursorShape = CursorShape.Arrow;
				break;
			case InputEventMouseMotion motion:
				if (_dragging) {
					Position += motion.Position - _relativePos;
				}

				break;
		}
	}

	private void OnResized(InputEvent @event, ResizeDirection resizeDirection) {
		switch (@event) {
			case InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true } mouseButton:
				_resizable = true;
				_relativePos = mouseButton.Position;
				break;
			case InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false }:
				_resizable = false;
				break;
			case InputEventMouseMotion motion:
				if (_resizable) {
					ResizeWindow(resizeDirection, motion.Position - _relativePos);
				}

				break;
		}
	}
}