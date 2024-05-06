namespace 创世记;

public static partial class Log {
	public readonly record struct LogInfo(string Message, Severity Severity, string Date, string? WorldName, int Ratio = 100) {
		public readonly string LogText = $"[{Date}]{(WorldName == null ? string.Empty : $"[{WorldName}]")}[{Severity}] {Message}";
	}
}