using System;
using System.Collections.Generic;
using Godot;
// ReSharper disable HeuristicUnreachableCode
#pragma warning disable CS0162 // 检测到不可到达的代码

namespace 创世记;

public static partial class Log {
	public enum Severity {
		Debug = -1,
		Info = 0,
		Warn = 1,
		Error = 2,
	}

	public const Severity LogSeverity = Severity.Debug;
	public static readonly List<LogData> LogList = [];

	static private void _log(LogData data) {
		LogList.Add(data);
		Utils.Tree.Root.SyncPost(_ => {
			LogWindow.TryAddItem(data);
			if (data.Severity != Severity.Error) return;
			ShowWindow();
			LogWindow.ScrollLog(data);
		});
		if (LogSeverity > data.Severity) return;
		var str = data.LogText;
		switch (data.Severity) {
			case Severity.Warn:
				GD.PushWarning(str);
				GD.PrintRich($"[color=orange]{str}[/color]");
				break;
			case Severity.Error:
				GD.PrintRich($"[color=red]{str}[/color]");
				break;
			case Severity.Info:
				GD.Print(str);
				break;
			case Severity.Debug:
			default:
				GD.PrintRich($"[color=gray]{str}[/color]");
				break;
		}
	}

	static private void _log(string m, Severity s) {
		ArgumentNullException.ThrowIfNull(m);
		var data = new LogData(m, s, DateTime.Now.ToString("MM-dd HH:mm:ss.fff"), Main.CurrentWorldInfo);
		_log(data);
	}

	public static void ShowWindow() {
		LogWindow.Launch();
	}

	public static void Debug(params string[] m) {
		_log(m.Join(" "), Severity.Debug);
	}

	public static void Info(params string[] m) {
		_log(m.Join(" "), Severity.Info);
	}

	public static void Warn(params string[] m) {
		_log(m.Join(" "), Severity.Warn);
	}

	public static void Error(params string[] m) {
		_log(m.Join(" "), Severity.Error);
		if (Utils.Tree.CurrentScene is not Main main) return;
		main.ExitWorld(1);
	}
}