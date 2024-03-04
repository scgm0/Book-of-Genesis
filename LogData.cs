using System;
using Godot;

namespace 创世记;

public static partial class Log {
	public sealed record LogData(string Message, Severity Severity, string Time, WorldInfo? WorldInfo) {
		public readonly string LogText = $"[{Time}]{(WorldInfo == null ? string.Empty : $"[{WorldInfo.Name}]")}[{Severity}] {Message}";
	}
}