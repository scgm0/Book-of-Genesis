using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using FileAccess = Godot.FileAccess;
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
	private const int MaxLogCount = 5;
	static private StreamWriter? _logWriter;

	static Log() {
		if (!FileAccess.FileExists(Utils.LogPath)) return;
		var logDir = Utils.LogPath.GetBaseDir();
		var logName = Utils.LogPath.GetFile().GetBaseName();
		var logExt = Utils.LogPath.GetExtension();
		DirAccess.RenameAbsolute(Utils.LogPath, $"{logDir}/{logName}{DateTime.Now:yyyy-MM-ddTHH.mm.ss}.{logExt}");
		var logFiles = Directory.GetFiles(logDir, $"{logName}*.{logExt}").ToList();
		while (logFiles.Count > MaxLogCount) {
			DirAccess.RemoveAbsolute(logFiles[0]);
			logFiles.RemoveAt(0);
		}
	}

	static private void _log(LogInfo info) {
		LogList.Add(info);
		LogWindow.TryAddItem(info);

		if (LogSeverity > info.Severity) return;
		var str = info.LogText;
		var old = Console.ForegroundColor;
		switch (info.Severity) {
			case Severity.Trace:
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(str);
				break;
			case Severity.Error:
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(str);
				LogWindow.ToggleVisible(true);
				LogWindow.ScrollLog(info);
				break;
			case Severity.Warn:
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(str);
				break;
			case Severity.Info:
				Console.WriteLine(str);
				break;
			case Severity.Debug:
			default:
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.WriteLine(str);
				break;
		}

		Console.ForegroundColor = old;

		using (_logWriter = new StreamWriter(Utils.LogPath, true)) {
			_logWriter.WriteLineAsync(info.LogText);
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