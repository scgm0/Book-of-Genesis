using Godot;

// ReSharper disable MemberCanBePrivate.Global

namespace 创世记;

[Tool]
[GlobalClass]
public partial class SmoothScroll : ScrollContainer {

	public enum ScrollType { Wheel, Bar, Drag }

	private float _bottomDistance;
	private bool _contentDragging;
	private Control _contentNode = null!;
	private float _damping = 0.1f;

	private Vector2 _dragRelative = Vector2.Zero;
	private Vector2 _dragStartPos = Vector2.Zero;
	private float[] _dragTempData = [];
	private float _friction = 0.9f;
	private bool _hideScrollbarOverTime;
	private bool _hScrollbarDragging;
	private bool _isScrolling;
	private float _justSnapUnder = 0.4f;
	private float _justStopUnder = 0.01f;

	private ScrollType _lastScrollType;
	private float _leftDistance;
	private Vector2 _pos = new(0, 0);
	private float _rightDistance;
	private Timer? _scrollbarHideTimer = new();
	private Tween? _scrollbarHideTween;
	private float _topDistance;

	private Vector2 _velocity = new(0, 0);
	private bool _vScrollbarDragging;

	[Export]
	public bool AllowHorizontalScroll = true;

	[Export]
	public bool AllowOverDragging = true;

	[Export]
	public bool AllowVerticalScroll = true;

	[Export]
	public bool AutoAllowScroll = true;

	[Export(PropertyHint.Range, "0, 1")]
	public float DampingDrag = 0.1f;

	[Export(PropertyHint.Range, "0, 1")]
	public float DampingScroll = 0.1f;

	[Export]
	public bool DebugMode;

	[Export(PropertyHint.Range, "0, 50")]
	public int FollowFocusMargin = 20;

	[Export(PropertyHint.Range, "0, 1")]
	public float FrictionDrag = 0.9f;

	[Export(PropertyHint.Range, "0, 1")]
	public float FrictionScroll = 0.9f;

	[Export]
	public float ScrollbarFadeInTime = 0.2f;

	[Export]
	public float ScrollbarFadeOutTime = 0.5f;

	[Export]
	public float ScrollbarHideTime = 5.0f;

	[Export(PropertyHint.Range, "0, 10, 0.01, or_greater")]
	public float Speed = 5.0f;

	[Export]
	public bool HideScrollbarOverTime {
		get => _hideScrollbarOverTime;
		set => _hideScrollbarOverTime = SetHideScrollbarOverTime(value);
	}

	public override void _Ready() {
		GetVScrollBar().Scrolling += _OnVScrollBarScrolling;
		GetHScrollBar().Scrolling += _OnHScrollBarScrolling;
		GetVScrollBar().GuiInput += _ScrollbarInput;
		GetHScrollBar().GuiInput += _ScrollbarInput;
		GetViewport().GuiFocusChanged += _OnFocusChanged;

		foreach (var c in GetChildren()) {
			if (c is not ScrollBar) {
				_contentNode = (Control)c;
			}
		}

		AddChild(_scrollbarHideTimer);
		_scrollbarHideTimer!.Timeout += _ScrollbarHideTimerTimeout;
		if (HideScrollbarOverTime) {
			_scrollbarHideTimer.Start(ScrollbarHideTime);
		}

		GetTree().NodeAdded += _OnNodeAdded;
	}

	public override void _Process(double delta) {
		if (Engine.IsEditorHint()) {
			return;
		}

		CalculateDistance();
		Scroll(true, _velocity.Y, _pos.Y, (float)delta);
		Scroll(false, _velocity.X, _pos.X, (float)delta);
		GetVScrollBar().SetValueNoSignal(-_pos.Y);
		GetVScrollBar().QueueRedraw();
		GetHScrollBar().SetValueNoSignal(-_pos.X);
		GetHScrollBar().QueueRedraw();
		UpdateState();

		if (DebugMode) {
			QueueRedraw();
		}
	}

	private void _ScrollbarInput(InputEvent ev) {
		if (HideScrollbarOverTime) {
			ShowScrollbars();
			_scrollbarHideTimer?.Start(ScrollbarHideTime);
		}

		if (ev is not InputEventMouseButton mouseButton) return;
		if (mouseButton.ButtonIndex is MouseButton.WheelDown or MouseButton.WheelUp or MouseButton.WheelLeft
			or MouseButton.WheelRight) {
			_GuiInput(mouseButton);
		}
	}

	public override void _GuiInput(InputEvent @event) {
		if (_hideScrollbarOverTime) {
			ShowScrollbars();
			_scrollbarHideTimer?.Start(ScrollbarHideTime);
		}

		_vScrollbarDragging = GetVScrollBar().HasFocus();
		_hScrollbarDragging = GetHScrollBar().HasFocus();

		switch (@event) {
			case InputEventMouseButton mouseButtonEvent:
				switch (mouseButtonEvent.ButtonIndex) {
					case MouseButton.WheelDown:
						HandleMouseButtonWheel(ScrollType.Wheel, -Speed, mouseButtonEvent.ShiftPressed);
						break;
					case MouseButton.WheelUp:
						HandleMouseButtonWheel(ScrollType.Wheel, Speed, mouseButtonEvent.ShiftPressed);
						break;
					case MouseButton.WheelLeft:
						HandleMouseButtonWheel(ScrollType.Wheel, -Speed, mouseButtonEvent.ShiftPressed);
						break;
					case MouseButton.WheelRight:
						HandleMouseButtonWheel(ScrollType.Wheel, Speed, mouseButtonEvent.ShiftPressed);
						break;
					case MouseButton.Left:
						HandleMouseButtonLeft(mouseButtonEvent.Pressed, mouseButtonEvent.Position);
						break;
				}

				break;
			case InputEventScreenDrag screenDragEvent:
				HandleMouseMotion(screenDragEvent.Position);
				break;
			case InputEventMouseMotion mouseMotion:
				HandleMouseMotion(mouseMotion.Position);
				break;
			case InputEventScreenTouch screenTouchEvent:
				HandleScreenTouch(screenTouchEvent.Pressed, screenTouchEvent.Position);
				break;
		}

		GetTree().Root.SetInputAsHandled();
		@event.Dispose();
	}

	private void HandleMouseButtonWheel(ScrollType scrollType, float scrollSpeed, bool shiftPressed) {
		if (_lastScrollType != scrollType) return;
		if (shiftPressed) {
			if (ShouldScrollHorizontal())
				_velocity.X += scrollSpeed;
		} else {
			if (ShouldScrollVertical())
				_velocity.Y += scrollSpeed;
		}

		_friction = FrictionScroll;
		_damping = DampingScroll;
	}

	private void HandleMouseButtonLeft(bool pressed, Vector2 pos) {
		if (pressed) {
			_contentDragging = true;
			_lastScrollType = ScrollType.Drag;
			_friction = 0.0f;
			_dragStartPos = _contentNode.Position;
			_dragRelative = pos;
			InitDragTempData();
		} else {
			_contentDragging = false;
			_friction = FrictionDrag;
			_damping = DampingDrag;
		}
	}

	private void HandleMouseMotion(Vector2 pos) {
		if (!_contentDragging) return;
		_isScrolling = true;
		if (ShouldScrollHorizontal()) {
			_dragTempData[0] += pos.X - _dragRelative.X;
		}

		if (ShouldScrollVertical()) {
			_dragTempData[1] += pos.Y - _dragRelative.Y;
		}

		_dragRelative = pos;
		RemoveAllChildrenFocus(this);
		HandleContentDragging();
	}

	private void HandleScreenTouch(bool pressed, Vector2 pos) {
		if (pressed) {
			_contentDragging = true;
			_lastScrollType = ScrollType.Drag;
			_friction = 0.0f;
			_dragStartPos = _contentNode.Position;
			_dragRelative = pos;
			InitDragTempData();
		} else {
			_contentDragging = false;
			_friction = FrictionDrag;
			_damping = DampingDrag;
		}
	}

	private void _OnFocusChanged(Control control) {
		var isChild = _contentNode.IsAncestorOf(control);

		if (!isChild || !FollowFocus) {
			return;
		}

		var focusSizeX = control.GetRect().Size.X;
		var focusSizeY = control.GetRect().Size.Y;
		var focusLeft = control.GlobalPosition.X - GlobalPosition.X;
		var focusRight = focusLeft + focusSizeX;
		var focusTop = control.GlobalPosition.Y - GlobalPosition.Y;
		var focusBottom = focusTop + focusSizeY;

		if (focusTop < 0.0f) {
			ScrollYTo(_contentNode.Position.Y - focusTop + FollowFocusMargin);
		}

		if (focusBottom > GetRect().Size.Y) {
			ScrollYTo(_contentNode.Position.Y - focusBottom + GetRect().Size.Y - FollowFocusMargin);
		}

		if (focusLeft < 0.0f) {
			ScrollXTo(_contentNode.Position.X - focusLeft + FollowFocusMargin);
		}

		if (focusRight > GetRect().Size.X) {
			ScrollXTo(_contentNode.Position.X - focusRight + GetRect().Size.X - FollowFocusMargin);
		}
	}

	private void _OnVScrollBarScrolling() {
		_vScrollbarDragging = true;
		_lastScrollType = ScrollType.Bar;
	}

	private void _OnHScrollBarScrolling() {
		_hScrollbarDragging = true;
		_lastScrollType = ScrollType.Bar;
	}

	public void _OnNodeAdded(Node node) {
		if (node is not Control control || !Engine.IsEditorHint()) return;
		if (IsAncestorOf(control)) {
			control.MouseFilter = MouseFilterEnum.Pass;
		}
	}

	public void _ScrollbarHideTimerTimeout() {
		if (!AnyScrollBarDragged()) {
			HideScrollbars();
		}
	}

	public bool SetHideScrollbarOverTime(bool value) {
		if (value == false) {
			_scrollbarHideTimer?.Stop();

			_scrollbarHideTimer?.QueueFree();

			GetHScrollBar().Modulate = Colors.White;
			GetVScrollBar().Modulate = Colors.White;
		} else {
			if (_scrollbarHideTimer != null && _scrollbarHideTimer.IsInsideTree()) {
				_scrollbarHideTimer.Start(ScrollbarHideTime);
			}
		}

		return value;
	}

	private void Scroll(bool vertical, float axisVelocity, float axisPos, float delta) {
		if (vertical) {
			if (!ShouldScrollVertical()) {
				return;
			}
		} else {
			if (!ShouldScrollHorizontal()) {
				return;
			}
		}

		if (Mathf.Abs(axisVelocity) <= _justStopUnder) {
			axisVelocity = 0.0f;
		}

		if (!_contentDragging) {
			var result = HandleOverDrag(vertical, axisVelocity, axisPos);
			axisVelocity = result[0];
			axisPos = result[1];
			axisPos += axisVelocity * (Mathf.Pow(_friction, delta * 100) - 1) / Mathf.Log(_friction);
			axisVelocity *= Mathf.Pow(_friction, delta * 100);
		}

		if (HandleScrollbarDrag()) {
			return;
		}

		if (vertical) {
			if (!AllowOverDragging) {
				if (IsOutsideTopBoundary(axisPos)) {
					axisPos = 0.0f;
					axisVelocity = 0.0f;
				} else if (IsOutsideBottomBoundary(axisPos)) {
					axisPos = Size.Y - _contentNode.Size.Y;
					axisVelocity = 0.0f;
				}
			}

			_contentNode.Position = _contentNode.Position with { Y = axisPos };
			_pos.Y = axisPos;
			_velocity.Y = axisVelocity;
		} else {
			if (!AllowOverDragging) {
				if (IsOutsideLeftBoundary(axisPos)) {
					axisPos = 0.0f;
					axisVelocity = 0.0f;
				} else if (IsOutsideRightBoundary(axisPos)) {
					axisPos = Size.X - _contentNode.Size.X;
					axisVelocity = 0.0f;
				}
			}

			_contentNode.Position = _contentNode.Position with { X = axisPos };
			_pos.X = axisPos;
			_velocity.X = axisVelocity;
		}
	}

	public float[] HandleOverDrag(bool vertical, float axisVelocity, float axisPos) {
		var dist1 = vertical ? _topDistance : _leftDistance;
		var dist2 = vertical ? _bottomDistance : _rightDistance;

		if (vertical) {
			var sizeY = Size.Y;
			if (GetHScrollBar().Visible) {
				sizeY -= GetHScrollBar().Size.Y;
			}

			dist2 += Mathf.Max(sizeY - _contentNode.Size.Y, 0);
		} else {
			var sizeX = _contentNode.Size.X;
			if (GetVScrollBar().Visible) {
				sizeX -= GetVScrollBar().Size.X;
			}

			dist2 += Mathf.Max(sizeX - _contentNode.Size.X, 0);
		}

		float[] result = [axisVelocity, axisPos];

		if (!(dist1 > 0 || dist2 < 0) || WillStopWithin(vertical, axisVelocity)) {
			return result;
		}

		if (dist1 > 0) {
			if (dist1 < _justSnapUnder && Mathf.Abs(axisVelocity) < _justSnapUnder) {
				result[0] = 0.0f;
				result[1] -= dist1;
			} else {
				result[0] = Calculate(dist1);
			}
		} else if (dist2 < 0) {
			if (dist2 > -_justSnapUnder && Mathf.Abs(axisVelocity) < _justSnapUnder) {
				result[0] = 0.0f;
				result[1] -= dist2;
			} else {
				result[0] = Calculate(dist2);
			}
		}

		return result;

		float Calculate(float dist) {
			axisVelocity = Mathf.Lerp(axisVelocity, -dist / 8 * (float)GetProcessDeltaTime() * 100, _damping);
			if (WillStopWithin(vertical, axisVelocity)) {
				axisVelocity = -dist * (1 - _friction) / (1 - Mathf.Pow(_friction, StopFrame(axisVelocity)));
			}

			return axisVelocity;
		}
	}

	public bool HandleScrollbarDrag() {
		if (_hScrollbarDragging) {
			_velocity.X = 0.0f;
			_pos.X = _contentNode.Position.X;
			return true;
		}

		if (!_vScrollbarDragging) return false;
		_velocity.Y = 0.0f;
		_pos.Y = _contentNode.Position.Y;
		return true;
	}

	public void HandleContentDragging() {
		if (ShouldScrollVertical()) {
			var yPos = CalculatePosition(_dragTempData[2], _dragTempData[3], _dragTempData[1]) + _dragStartPos.Y;
			_velocity.Y = (yPos - _pos.Y) / (float)GetProcessDeltaTime() / 100f;
			_pos.Y = yPos;
		}

		if (!ShouldScrollHorizontal()) return;
		var xPos = CalculatePosition(_dragTempData[4], _dragTempData[5], _dragTempData[0]) + _dragStartPos.X;
		_velocity.X = (xPos - _pos.X) / (float)GetProcessDeltaTime() / 100f;
		_pos.X = xPos;
		return;

		float CalculateDest(float delta, float damping) {
			if (delta >= 0.0f)
				return delta / (1 + delta * damping * 0.1f);
			return delta;
		}

		float CalculatePosition(float tempDist1, float tempDist2, float tempRelative) {
			if (tempRelative + tempDist1 > 0.0f) {
				var delta = Mathf.Min(tempRelative, tempRelative + tempDist1);
				var dest = CalculateDest(delta, DampingDrag);
				return dest - Mathf.Min(0.0f, tempDist1);
			}

			if (!(tempRelative + tempDist2 < 0.0f)) return tempRelative;
			{
				var delta = Mathf.Max(tempRelative, tempRelative + tempDist2);
				var dest = -CalculateDest(-delta, DampingDrag);
				return dest - Mathf.Max(0.0f, tempDist2);
			}
		}
	}

	public void CalculateDistance() {
		_bottomDistance = _contentNode.Position.Y + _contentNode.GetRect().Size.Y - GetRect().Size.Y;
		_topDistance = _contentNode.Position.Y;
		_rightDistance = _contentNode.Position.X + _contentNode.GetRect().Size.X - GetRect().Size.X;
		_leftDistance = _contentNode.Position.X;

		if (GetVScrollBar().Visible) {
			_rightDistance += GetVScrollBar().GetRect().Size.X;
		}

		if (GetHScrollBar().Visible) {
			_bottomDistance += GetHScrollBar().GetRect().Size.Y;
		}
	}

	public float StopFrame(float vel) {
		return Mathf.Floor(Mathf.Max(Mathf.Log(_justStopUnder / (Mathf.Abs(vel) + 0.001f)) / Mathf.Log(_friction * 0.999f),
			0.0f));
	}

	public bool WillStopWithin(bool vertical, float vel) {
		var stopFrame = StopFrame(vel);

		var stopDistance = vel * (1 - Mathf.Pow(_friction, stopFrame)) / (1 - _friction);
		var stopPos = vertical ? _pos.Y + stopDistance : _pos.X + stopDistance;

		var diff = vertical ? Size.Y - _contentNode.Size.Y : Size.X - _contentNode.Size.X;

		return stopPos <= 0.0f && stopPos >= Mathf.Min(diff, 0.0f);
	}

	public static void RemoveAllChildrenFocus(Node node) {
		if (node is Control control) {
			control.ReleaseFocus();
		}

		foreach (var child in node.GetChildren()) {
			RemoveAllChildrenFocus(child);
		}
	}

	public void UpdateState() {
		if (_contentDragging || _vScrollbarDragging || _hScrollbarDragging || _velocity != Vector2.Zero) {
			_isScrolling = true;
		} else {
			_isScrolling = false;
		}
	}

	public void InitDragTempData() {
		_dragTempData = [0.0f, 0.0f, _topDistance, _bottomDistance, _leftDistance, _rightDistance];
	}

	public void ScrollXTo(float xPos, float duration = 0.5f) {
		if (!ShouldScrollHorizontal()) return;

		_velocity.X = 0.0f;
		xPos = Mathf.Clamp(xPos, Size.X - _contentNode.Size.X, 0.0f);

		if (duration <= 0) {
			_pos.X = xPos;
			return;
		}

		var tween = CreateTween();
		tween.TweenProperty(this, "pos:x", xPos, duration).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quint);
	}

	public void ScrollYTo(float yPos, float duration = 0.5f) {
		if (!ShouldScrollVertical()) return;

		_velocity.Y = 0.0f;
		yPos = Mathf.Clamp(yPos, Size.Y - _contentNode.Size.Y, 0.0f);

		if (duration <= 0) {
			_pos.Y = yPos;
			return;
		}

		var tween = CreateTween();
		tween.TweenProperty(this, "pos:y", yPos, duration).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quint);
	}

	public void ScrollPageUp(float duration = 0.5f) {
		var destination = _contentNode.Position.Y + Size.Y;
		ScrollYTo(destination, duration);
	}


	public void ScrollPageDown(float duration = 0.5f) {
		var destination = _contentNode.Position.Y - Size.Y;
		ScrollYTo(destination, duration);
	}

	public void ScrollPageLeft(float duration = 0.5f) {
		var destination = _contentNode.Position.X + Size.X;
		ScrollXTo(destination, duration);
	}

	public void ScrollPageRight(float duration = 0.5f) {
		var destination = _contentNode.Position.X - Size.X;
		ScrollXTo(destination, duration);
	}

	public void ScrollVertically(float amount) { _velocity.Y -= amount; }

	public void ScrollHorizontally(float amount) { _velocity.X -= amount; }

	public void ScrollToTop(float duration = 0.5f) { ScrollYTo(0.0f, duration); }

	public void ScrollToBottom(float duration = 0.5f) { ScrollYTo(Size.Y - _contentNode.Size.Y, duration); }

	public void ScrollToLeft(float duration = 0.5f) { ScrollXTo(0.0f, duration); }

	public void ScrollToRight(float duration = 0.5f) { ScrollXTo(Size.X - _contentNode.Size.X, duration); }

	public bool IsOutsideTopBoundary(float yPos = default) {
		if (yPos == 0) {
			yPos = _pos.Y;
		}

		return yPos > 0.0f;
	}

	public bool IsOutsideBottomBoundary(float yPos = default) {
		if (yPos == 0) {
			yPos = _pos.Y;
		}

		return yPos < Size.Y - _contentNode.Size.Y;
	}

	public bool IsOutsideLeftBoundary(float xPos = default) {
		if (xPos == 0) {
			xPos = _pos.X;
		}

		return xPos > 0.0f;
	}

	public bool IsOutsideRightBoundary(float xPos = default) {
		if (xPos == 0) {
			xPos = _pos.X;
		}

		return xPos < Size.X - _contentNode.Size.X;
	}

	public bool AnyScrollBarDragged() {
		if (GetVScrollBar() == null) return GetHScrollBar().HasFocus();
		return GetVScrollBar().HasFocus() || GetHScrollBar().HasFocus();
	}

	public bool ShouldScrollVertical() {
		var disableScroll = _contentNode.Size.Y - Size.Y < 1 || (!AllowVerticalScroll && AutoAllowScroll) ||
			!AllowVerticalScroll;
		if (!disableScroll) return true;
		_velocity.Y = 0.0f;
		return false;
	}

	public bool ShouldScrollHorizontal() {
		var disableScroll = _contentNode.Size.X - Size.X < 1 || (!AllowHorizontalScroll && AutoAllowScroll) ||
			!AllowHorizontalScroll;
		if (!disableScroll) return true;
		_velocity.X = 0.0f;
		return false;
	}

	public void HideScrollbars() {
		_scrollbarHideTween?.Kill();
		_scrollbarHideTween = CreateTween();
		_scrollbarHideTween.SetParallel();
		_scrollbarHideTween.TweenProperty(GetVScrollBar(), "modulate", Colors.Transparent, ScrollbarFadeOutTime);
		_scrollbarHideTween.TweenProperty(GetHScrollBar(), "modulate", Colors.Transparent, ScrollbarFadeOutTime);
	}

	public void ShowScrollbars() {
		_scrollbarHideTween?.Kill();
		_scrollbarHideTween = CreateTween();
		_scrollbarHideTween.SetParallel();
		_scrollbarHideTween.TweenProperty(GetVScrollBar(), "modulate", Colors.White, ScrollbarFadeInTime);
		_scrollbarHideTween.TweenProperty(GetHScrollBar(), "modulate", Colors.White, ScrollbarFadeInTime);
	}
}