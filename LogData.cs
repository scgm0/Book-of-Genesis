using System;
using Godot;

namespace 创世记;

public static partial class Log {
	public readonly record struct LogData(string Message, Severity Severity, string Time, string? WorldName, int Ratio = 100) {
		public readonly string LogText = $"[{Time}]{(WorldName == null ? string.Empty : $"[{WorldName}]")}[{Severity}] {Message}";
	}
}