using System;
using System.Collections.Generic;
using Godot;
// ReSharper disable UnusedMember.Global
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

namespace 创世记;

public static partial class Log {
	public sealed partial class LogWindow : ControlWindow {
		static private LogWindow? _instance;

		internal static void Launch(bool show = true) {
			if (_instance != null) {
				_instance.Show();
				_instance.ProcessMode = ProcessModeEnum.Always;
				return;
			}

			_instance = CreateInstance();
			_instance.Visible = show;

			foreach (var logData in LogList) {
				TryAddItem(logData);
			}

			Utils.Tree.Root.CallDeferred(Node.MethodName.AddChild, _instance);
			// Utils.Tree.Root.CallDeferred(Node.MethodName.MoveChild, _instance, 0);
		}

		internal static void ScrollLog(LogData data) {
			if (_instance is null) return;
			if (!_instance._logDataMap.TryGetValue(data, out var item)) {
				item = TryAddItem(data);
			}

			_instance._tree.ScrollToItem(item);
			_instance._tree.SetSelected(item, 3);
		}

		internal static TreeItem TryAddItem(LogData logData) {
			if (_instance is null) {
				Launch(false);
			}

			lock (_instance!) {
				var logDataMap = _instance._logDataMap;
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

					logDataMap.Add(logData, treeItem);
					treeItemMap.Add(treeItem, logData);
				}

				UpdateTreeItem(logData, treeItem);
				return treeItem;
			}
		}

		internal static void TryRemoveItem(LogData logData) {
			if (_instance is null) return;
			lock (_instance) {
				var logDataMap = _instance._logDataMap;
				var treeItemMap = _instance._treeItemMap;

				if (!logDataMap.Remove(logData, out var treeItem)) return;
				treeItemMap.Remove(treeItem);

				treeItem.Free();
			}
		}

		static private void UpdateTreeItem(LogData logData, TreeItem treeItem) {
			treeItem.SetText(0, logData.Time);
			treeItem.SetText(1, logData.WorldInfo?.Name ?? string.Empty);
			treeItem.SetText(2, logData.Severity.ToString());
			treeItem.SetText(3, logData.Message.Replace("\n", string.Empty));
			switch (logData.Severity) {
				case Severity.Debug:
					treeItem.SetCustomColor(2, Colors.Gray);
					break;
				case Severity.Info:
					treeItem.SetCustomColor(2, Colors.White);
					break;
				case Severity.Warn:
					treeItem.SetCustomColor(2, Colors.Orange);
					break;
				case Severity.Error:
					treeItem.SetCustomColor(2, Colors.Red);
					break;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		static private LogWindow CreateInstance() {
			var tree = new Tree {
				SizeFlagsHorizontal = SizeFlags.Fill,
				SizeFlagsVertical = SizeFlags.Fill,
				SelectMode = Tree.SelectModeEnum.Row,
				Columns = 4,
				ColumnTitlesVisible = true,
				HideRoot = true
			};
			var textEdit = new TextEdit { Editable = false };
			textEdit.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());

			var instance = new LogWindow(tree, textEdit) {
				Title = "创世记 日志",
				CustomMinimumSize = new Vector2(800, 600)
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
								var spacer = new Control {
									SizeFlagsHorizontal = SizeFlags.ExpandFill
								};

								var openButton = new Button {
									Text = "打开日志文件"
								};

								openButton.Pressed += () => OS.ShellOpen("file://" + ProjectSettings.GlobalizePath(ProjectSettings.GetSetting("debug/file_logging/log_path").ToString()));

								toolbar.AddChild(spacer);
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
								tree.SetColumnTitle(3, "消息");

								tree.SetColumnExpandRatio(0, 2);
								tree.SetColumnExpandRatio(1, 1);
								tree.SetColumnExpandRatio(2, 1);
								tree.SetColumnExpandRatio(3, 8);

								tree.SetColumnClipContent(0, true);
								tree.SetColumnClipContent(1, true);
								tree.SetColumnClipContent(2, true);
								tree.SetColumnClipContent(3, true);

								tree.ItemSelected += () => {
									var item = tree.GetSelected();
									if (!instance._treeItemMap.TryGetValue(item, out var logData)) return;
									textEdit.Text = logData.LogText;
									textEdit.AddThemeColorOverride("font_readonly_color", item.GetCustomColor(2));
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
		}

		public LogWindow() { }

		public override void _Notification(int what) {
			if (what != NotificationWMCloseRequest) return;
			Hide();
			ProcessMode = ProcessModeEnum.Disabled;
		}
	}
}