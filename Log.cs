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
		Trace = 3
	}

	public const Severity LogSeverity = Severity.Debug;
	public static readonly List<LogInfo> LogList = [];

	static private void _log(LogInfo info) {

		LogList.Add(info);
		LogWindow.TryAddItem(info);

		if (LogSeverity > info.Severity) return;
		var str = info.LogText;
		switch (info.Severity) {
			case Severity.Trace:
				GD.PrintRich($"[color=green]{str}[/color]");
				break;
			case Severity.Error:
				GD.PrintRich($"[color=red]{str}[/color]");
				LogWindow.ToggleVisible(true);
				LogWindow.ScrollLog(info);
				break;
			case Severity.Warn:
				GD.PrintRich($"[color=yellow]{str}[/color]");
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
		var data = new LogInfo(m, s, DateTime.Now.ToString("MM-dd HH:mm:ss.fff"), Main.CurrentWorldInfo?.Name);
		_log(data);
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
		if (Utils.Tree.CurrentScene is not Main) return;
		World.Instance?.Exit(1);
	}

	public static void Trace(params string[] m) {
		_log(m.Join(" "), Severity.Trace);
	}

	public static void Assert(bool condition, params string[] m) {
		if (condition) return;
		Error(m);
	}
}