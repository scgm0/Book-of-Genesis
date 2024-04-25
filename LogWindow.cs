using System;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp;
using Godot;
// ReSharper disable UnusedMember.Global
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

namespace 创世记;

public static partial class Log {
	public sealed partial class LogWindow : ControlWindow {
		static private LogWindow? _instance;

		static private readonly Texture2D DebugIcon = GD.Load<Texture2D>("res://Assets/Icon/Debug.svg");
		static private readonly Texture2D InfoIcon = GD.Load<Texture2D>("res://Assets/Icon/Info.svg");
		static private readonly Texture2D WarnIcon = GD.Load<Texture2D>("res://Assets/Icon/Warn.svg");
		static private readonly Texture2D ErrorIcon = GD.Load<Texture2D>("res://Assets/Icon/Error.svg");
		static private readonly Texture2D TraceIcon = GD.Load<Texture2D>("res://Assets/Icon/Trace.svg");


		public static void ToggleVisible() => ToggleVisible(null);

		public static void ToggleVisible(bool? visible) {
			if (_instance is null) {
				Launch();
			}

			visible ??= !_instance!.Visible;

			if (!(bool)visible) {
				_instance!.Hide();
				_instance.ProcessMode = ProcessModeEnum.Disabled;
			} else {
				_instance!.Show();
				_instance.ProcessMode = ProcessModeEnum.Always;
				if (LogList.Count > 0) {
					ScrollLog(LogList[^1]);
				}
			}
		}

		static private void Launch() {
			if (_instance != null) return;

			_instance = CreateInstance();
			_instance.Visible = false;

			foreach (var logData in LogList) {
				TryAddItem(logData);
			}

			var canvas = new CanvasLayer();
			canvas.Layer = 50;
			canvas.AddChild(_instance);
			Utils.Tree.Root.AddChild(canvas);
			Utils.Tree.Root.MoveChild(canvas, 0);
		}

		static private void SearchLog(string text) {
			if (string.IsNullOrEmpty(text)) {
				SortLog();
				return;
			}

			var ratioList = LogList.Select(data => data with {
					Ratio = Fuzz.WeightedRatio(text, data.Message)
				})
				.OrderByDescending(data => data.Ratio)
				.ToList();
			SortLog(ratioList);
			if (ratioList[0].Ratio <= 0) return;
			ScrollLog(ratioList[0] with {
				Ratio = 100
			}, false);
		}

		static private void SortLog() {
			SortLog(LogList);
		}

		static private void SortLog(IEnumerable<LogInfo> logDatas) {
			_instance!._tree.DeselectAll();
			_instance._textEdit.Text = "";
			TreeItem? lastItem = null;
			foreach (var logData in logDatas) {
				var treeItem = _instance._logDataMap[logData with { Ratio = 100 }];
				treeItem.Visible = logData.Ratio > 0;
				if (lastItem == null) {
					lastItem = treeItem;
				} else {
					treeItem.MoveAfter(lastItem);
					lastItem = treeItem;
				}
			}
		}

		internal static void ScrollLog(LogInfo info, bool focus = true) {
			if (_instance is null) return;
			if (!_instance._logDataMap.TryGetValue(info, out var item)) {
				item = TryAddItem(info);
			}

			_instance._tree.ScrollToItem(item);
			if (!focus) return;
			_instance._tree.SetSelected(item, 3);
			_instance._tree.GrabFocus();
		}

		internal static TreeItem TryAddItem(LogInfo logInfo) {
			if (_instance is null) {
				Launch();
			}

			var logDataMap = _instance!._logDataMap;
			var treeItemMap = _instance._treeItemMap;
			var tree = _instance._tree;
			if (!logDataMap.TryGetValue(logInfo, out var treeItem)) {
				treeItem = tree.CreateItem(_instance._rootTreeItem);

				treeItem.SetTextAlignment(1, HorizontalAlignment.Center);
				treeItem.SetTextAlignment(2, HorizontalAlignment.Center);

				treeItem.SetCustomColor(0, Colors.White);
				treeItem.SetCustomColor(1, Colors.White);
				treeItem.SetCustomColor(2, Colors.White);
				treeItem.SetCustomColor(3, Colors.White);

				treeItem.SetIconMaxWidth(2, 24);

				logDataMap.Add(logInfo, treeItem);
				treeItemMap.Add(treeItem, logInfo);
			}

			UpdateTreeItem(logInfo, treeItem);
			return treeItem;
		}

		internal static void TryRemoveItem(LogInfo logInfo) {
			if (_instance is null) return;
			var logDataMap = _instance._logDataMap;
			var treeItemMap = _instance._treeItemMap;

			if (!logDataMap.Remove(logInfo, out var treeItem)) return;
			treeItemMap.Remove(treeItem);

			treeItem.Free();
		}

		static private void UpdateTreeItem(LogInfo logInfo, TreeItem treeItem) {
			treeItem.SetText(0, logInfo.Time);
			treeItem.SetText(1, logInfo.WorldName ?? string.Empty);
			treeItem.SetTooltipText(2, logInfo.Severity.ToString());
			treeItem.SetText(3, logInfo.Message.Replace("\n", string.Empty));
			switch (logInfo.Severity) {
				case Severity.Debug:
					treeItem.SetIcon(2, DebugIcon);
					treeItem.SetIconModulate(2, Colors.Gray);
					treeItem.SetCustomColor(3, Colors.Gray);
					break;
				case Severity.Info:
					treeItem.SetIcon(2, InfoIcon);
					treeItem.SetIconModulate(2, Colors.White);
					treeItem.SetCustomColor(3, Colors.White);
					break;
				case Severity.Warn:
					treeItem.SetIcon(2, WarnIcon);
					treeItem.SetIconModulate(2, Colors.Orange);
					treeItem.SetCustomColor(3, Colors.Orange);
					break;
				case Severity.Error:
					treeItem.SetIcon(2, ErrorIcon);
					treeItem.SetIconModulate(2, Colors.Red);
					treeItem.SetCustomColor(3, Colors.Red);
					break;
				case Severity.Trace:
					treeItem.SetIcon(2, TraceIcon);
					treeItem.SetIconModulate(2, Colors.Green);
					treeItem.SetCustomColor(3, Colors.Green);
					break;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		static private LogWindow CreateInstance() {
			using var monoFont = GD.Load<Font>("res://Assets/Font/Mono.tres");
			var tree = new Tree {
				Name = "Tree",
				SizeFlagsHorizontal = SizeFlags.Fill,
				SizeFlagsVertical = SizeFlags.Fill,
				SelectMode = Tree.SelectModeEnum.Row,
				Columns = 4,
				ColumnTitlesVisible = true,
				HideRoot = true
			};
			tree.AddThemeFontOverride("font", monoFont);
			tree.AddThemeFontOverride("title_button_font", monoFont);
			var textEdit = new TextEdit {
				Name = "Message",
				Editable = false,
				DrawTabs = true,
				DrawSpaces = true
			};
			textEdit.AddThemeFontOverride("font", monoFont);
			textEdit.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());
			textEdit.AddThemeStyleboxOverride("normal", new StyleBoxFlat {
				BgColor = Color.FromHtml("#1a1a1a99"),
				CornerRadiusTopRight = 3,
				CornerRadiusBottomRight = 3,
				CornerRadiusTopLeft = 3,
				CornerRadiusBottomLeft = 3,
				ContentMarginLeft = 8,
				ContentMarginRight = 8,
				ContentMarginTop = 8,
				ContentMarginBottom = 8
			});
			textEdit.AddThemeStyleboxOverride("read_only", new StyleBoxFlat {
				DrawCenter = false,
				ContentMarginLeft = 8,
				ContentMarginRight = 8,
				ContentMarginTop = 8,
				ContentMarginBottom = 8
			});

			var instance = new LogWindow(tree, textEdit) {
				Title = "创世记 日志",
#if GODOT_ANDROID
				ResizeGripExtra = 8,
#endif
				Size = new Vector2(800, 580),
				CustomMinimumSize = new Vector2(282, 316)
			};
			instance.DecorationsStyle.BgColor = new Color(0.113725f, 0.133333f, 0.160784f);
			instance.DecorationsStyle.CornerRadiusTopLeft = 8;
			instance.DecorationsStyle.CornerRadiusTopRight = 8;
			instance.DecorationsStyle.CornerRadiusBottomLeft = 8;
			instance.DecorationsStyle.CornerRadiusBottomRight = 8;
			instance.DecorationsStyle.BorderColor = new Color(1, 1, 1, 0.5f);
			instance.DecorationsStyle.BorderWidthLeft = 1;
			instance.DecorationsStyle.BorderWidthRight = 1;
			instance.DecorationsStyle.BorderWidthTop = 1;
			instance.DecorationsStyle.BorderWidthBottom = 1;
			{
				var panelContainer = new PanelContainer {
					SizeFlagsHorizontal = SizeFlags.ExpandFill,
					SizeFlagsVertical = SizeFlags.ExpandFill
				};
				panelContainer.AddThemeStyleboxOverride("panel", new StyleBoxEmpty());
				{
					var marginContainer = new MarginContainer();
					const int margin = 10;
					// marginContainer.AddThemeConstantOverride("margin_top", margin);
					marginContainer.AddThemeConstantOverride("margin_left", margin);
					marginContainer.AddThemeConstantOverride("margin_bottom", margin);
					marginContainer.AddThemeConstantOverride("margin_right", margin);
					{
						var vBox = new VBoxContainer {
							Alignment = BoxContainer.AlignmentMode.Begin
						};
						{
							var toolbar = new HBoxContainer();
							{
								var searchButton = new Button {
									Text = "搜索"
								};
								var searchBox = new LineEdit {
									SizeFlagsHorizontal = SizeFlags.ExpandFill,
									ClearButtonEnabled = true
								};
								var openButton = new Button {
									Text = "打开日志文件"
								};
								var clearButton = new Button {
									Text = "清除日志列表"
								};

								searchButton.Pressed += () => SearchLog(searchBox.Text);
								searchBox.TextChanged += text => {
									if (string.IsNullOrEmpty(text)) {
										SearchLog(text);
									}
								};
								searchBox.TextSubmitted += SearchLog;
								searchBox.VisibilityChanged += () => {
									if (!instance.Visible) {
										searchBox.Clear();
									}
								};
								openButton.Pressed += () => {
									OS.ShellOpen(Utils.LogPath);
								};
								clearButton.Pressed += () => {
									LogList.Clear();
									instance._treeItemMap.Clear();
									instance._logDataMap.Clear();
									tree.Clear();
									instance._rootTreeItem = tree.CreateItem();
								};

								toolbar.AddChild(searchButton);
								toolbar.AddChild(searchBox);
								toolbar.AddChild(openButton);
								toolbar.AddChild(clearButton);
							}
							vBox.AddChild(toolbar);
						}
						{
							var hSeparator = new HSeparator {
								SizeFlagsHorizontal = SizeFlags.ExpandFill
							};
							vBox.AddChild(hSeparator);
						}
						{
							var vSplitContainer = new VSplitContainer {
								SizeFlagsVertical = SizeFlags.ExpandFill,
								SplitOffset = 350
							};
							{
								tree.SetColumnTitle(0, "日期");
								tree.SetColumnTitle(1, "世界");
								tree.SetColumnTitle(2, "级别");
								tree.SetColumnTitle(3, "内容");

								tree.SetColumnExpandRatio(0, 2);
								tree.SetColumnExpandRatio(1, 1);
								tree.SetColumnExpandRatio(2, 0);
								tree.SetColumnExpandRatio(3, 8);

								tree.SetColumnClipContent(0, true);
								tree.SetColumnClipContent(1, true);
								tree.SetColumnClipContent(2, true);
								tree.SetColumnClipContent(3, true);

								tree.ItemSelected += () => {
									var item = tree.GetSelected();
									if (!instance._treeItemMap.TryGetValue(item, out var logData)) return;
									textEdit.Text = logData.LogText;
									textEdit.AddThemeColorOverride("font_readonly_color", item.GetIconModulate(2));
								};

								vSplitContainer.AddChild(tree);
								vSplitContainer.AddChild(textEdit);
							}
							vBox.AddChild(vSplitContainer);
						}
						marginContainer.AddChild(vBox);
					}
					panelContainer.AddChild(marginContainer);
				}

				instance.AddChild(panelContainer);
				panelContainer.SetAnchorsPreset(LayoutPreset.FullRect);
			}
			return instance;
		}

		private readonly Tree _tree;
		private TreeItem _rootTreeItem;
		private readonly TextEdit _textEdit;
		private readonly Dictionary<LogInfo, TreeItem> _logDataMap = new();
		private readonly Dictionary<TreeItem, LogInfo> _treeItemMap = new();

		private LogWindow(Tree tree, TextEdit textEdit) {
			_tree = tree;
			_textEdit = textEdit;
			_rootTreeItem = _tree.CreateItem();
			AddThemeStyleboxOverride("panel", new StyleBoxEmpty());
		}

		public LogWindow() { }

		public override void _Notification(int what) {
			if (what != NotificationWMCloseRequest) return;
			ToggleVisible(false);
		}
	}
}