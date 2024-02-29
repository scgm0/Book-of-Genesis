using System;
using Godot;
// ReSharper disable HeuristicUnreachableCode
#pragma warning disable CS0162 // 检测到不可到达的代码

namespace 创世记;

public static class Log {
	public enum Severity {
		Debug = -1,
		Info = 0,
		Warn = 1,
		Error = 2,
	}

	public const Severity LogSeverity = Severity.Debug;

	static private void _log(string m, Severity s = Severity.Debug) {
		ArgumentNullException.ThrowIfNull(m);
		if (LogSeverity > s) return;
		var message = $"[{DateTime.Now}] {(Main.CurrentWorldInfo == null ? string.Empty : $"[{Main.CurrentWorldInfo.Name}] ")}[{s}]: {m}";
		switch (s) {
			case Severity.Warn:
				GD.PushWarning(message);
				GD.PrintRich($"[color=orange]{message}[/color]");
				break;
			case Severity.Error:
				GD.PrintErr(message);
				break;
			case Severity.Info:
				GD.Print(message);
				break;
			case Severity.Debug:
			default:
				GD.PrintRich($"[code]{message}[/code]");
				break;
		}
	}

	public static void Debug(params string[] m) {
		_log(m.Join(" "));
	}

	public static void Info(params string[] m) {
		_log(m.Join(" "), Severity.Info);
	}

	public static void Warn(params string[] m) {
		_log(m.Join(" "), Severity.Warn);
	}

	public static void Error(params string[] m) {
		_log(m.Join(" "), Severity.Error);
	}
}