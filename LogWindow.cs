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

		static private readonly Dictionary<string, Texture2D> LogSeverityIcons = new() {
			{ "Debug", GD.Load<Texture2D>("res://Assets/Debug.svg") },
			{ "Info", GD.Load<Texture2D>("res://Assets/Info.svg") },
			{ "Warn", GD.Load<Texture2D>("res://Assets/Warn.svg") },
			{ "Error", GD.Load<Texture2D>("res://Assets/Error.svg") }
		};

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
				ScrollLog(LogList[^1]);
			}
		}

		internal static void Launch() {
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

		static private void SortLog(IEnumerable<LogData> logDatas) {
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

		internal static void ScrollLog(LogData data, bool focus = true) {
			if (_instance is null) return;
			if (!_instance._logDataMap.TryGetValue(data, out var item)) {
				item = TryAddItem(data);
			}

			_instance._tree.ScrollToItem(item);
			if (!focus) return;
			_instance._tree.SetSelected(item, 3);
			_instance._tree.GrabFocus();
		}

		internal static TreeItem TryAddItem(LogData logData) {
			if (_instance is null) {
				Launch();
			}

			var logDataMap = _instance!._logDataMap;
			var treeItemMap = _instance._treeItemMap;
			var tree = _instance._tree;
			if (!logDataMap.TryGetValue(logData, out var treeItem)) {
				treeItem = tree.CreateItem(_instance._rootTreeItem);

				treeItem.SetTextAlignment(1, HorizontalAlignment.Center);
				treeItem.SetTextAlignment(2, HorizontalAlignment.Center);

				treeItem.SetCustomColor(0, Colors.White);
				treeItem.SetCustomColor(1, Colors.White);
				treeItem.SetCustomColor(2, Colors.White);
				treeItem.SetCustomColor(3, Colors.White);

				treeItem.SetIconMaxWidth(2, 24);

				logDataMap.Add(logData, treeItem);
				treeItemMap.Add(treeItem, logData);
			}

			UpdateTreeItem(logData, treeItem);
			return treeItem;
		}

		internal static void TryRemoveItem(LogData logData) {
			if (_instance is null) return;
			var logDataMap = _instance._logDataMap;
			var treeItemMap = _instance._treeItemMap;

			if (!logDataMap.Remove(logData, out var treeItem)) return;
			treeItemMap.Remove(treeItem);

			treeItem.Free();
		}

		static private void UpdateTreeItem(LogData logData, TreeItem treeItem) {
			treeItem.SetText(0, logData.Time);
			treeItem.SetText(1, logData.WorldName ?? string.Empty);
			treeItem.SetTooltipText(2, logData.Severity.ToString());
			treeItem.SetText(3, logData.Message.Replace("\n", string.Empty));
			switch (logData.Severity) {
				case Severity.Debug:
					treeItem.SetIcon(2, LogSeverityIcons["Debug"]);
					treeItem.SetIconModulate(2, Colors.Gray);
					treeItem.SetCustomColor(3, Colors.Gray);
					break;
				case Severity.Info:
					treeItem.SetIcon(2, LogSeverityIcons["Info"]);
					treeItem.SetIconModulate(2, Colors.White);
					treeItem.SetCustomColor(3, Colors.White);
					break;
				case Severity.Warn:
					treeItem.SetIcon(2, LogSeverityIcons["Warn"]);
					treeItem.SetIconModulate(2, Colors.Orange);
					treeItem.SetCustomColor(3, Colors.Orange);
					break;
				case Severity.Error:
					treeItem.SetIcon(2, LogSeverityIcons["Error"]);
					treeItem.SetIconModulate(2, Colors.Red);
					treeItem.SetCustomColor(3, Colors.Red);
					break;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		static private LogWindow CreateInstance() {
			using var monoFont = GD.Load<Font>("res://Assets/Font/Mono.tres");
			var tree = new Tree {
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
				Editable = false,
				DrawTabs = true,
				DrawSpaces = true
			};
			textEdit.AddThemeFontOverride("font", monoFont);
			textEdit.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());

			var instance = new LogWindow(tree, textEdit) {
				Title = "创世记 日志",
#if GODOT_ANDROID
				ResizeGripExtra = 8,
#endif
				Size = new Vector2(800, 580),
				CustomMinimumSize = new Vector2(282, 316)
			};
			{
				var panelContainer = new PanelContainer {
					SizeFlagsHorizontal = SizeFlags.ExpandFill,
					SizeFlagsVertical = SizeFlags.ExpandFill
				};
				{
					var marginContainer = new MarginContainer();
					const int margin = 10;
					marginContainer.AddThemeConstantOverride("margin_top", margin);
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

								searchButton.Pressed += () => SearchLog(searchBox.Text);
								searchBox.TextChanged += SearchLog;
								searchBox.TextSubmitted += SearchLog;
								searchBox.VisibilityChanged += () => {
									if (!instance.Visible) {
										searchBox.Clear();
									}
								};
								openButton.Pressed += () => {
#if GODOT_ANDROID
									var path = Utils.LogPath;
#else
									var path = ProjectSettings.GlobalizePath(Utils.LogPath);
#endif
									OS.ShellOpen(path);
								};

								toolbar.AddChild(searchButton);
								toolbar.AddChild(searchBox);
								toolbar.AddChild(openButton);
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
		private readonly TreeItem _rootTreeItem;
		private readonly TextEdit _textEdit;
		private readonly Dictionary<LogData, TreeItem> _logDataMap = new();
		private readonly Dictionary<TreeItem, LogData> _treeItemMap = new();

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