#nullable enable
using System;
using System.Linq;
using SourceMaps;

namespace 创世记;

public static class StackTraceParser {

	public static string ReTrace(SourceMapCollection sourceMaps, string stacktrace, string? sourceRoot = null) {
		// 解析堆栈跟踪
		var trace = SourceMaps.StackTraces.StackTraceParser.Parse(stacktrace);

		// 遍历堆栈跟踪中的每一帧
		foreach (var frame in trace.Frames) {
			// 获取源映射
			var sourceMap = sourceMaps.GetSourceMapFor(frame.File);
			// 如果文件中包含问号，则获取没有问号部分的源映射
			if (sourceMap == null && frame.File?.IndexOf('?') >= 0)
				sourceMap = sourceMaps.GetSourceMapFor(frame.File[..frame.File.IndexOf('?')]);

			// 如果行号或列号不存在，则跳过
			if (frame.LineNumber == null || frame.ColumnNumber == null)
				continue;

			// 获取原始位置
			var originalPosition = sourceMap?.OriginalPositionFor(frame.LineNumber.Value, frame.ColumnNumber.Value);
			// 如果原始位置不存在，则跳过
			if (originalPosition == null)
				continue;
			// 更新文件名
			frame.File =
				new Uri(new Uri(frame.File!), originalPosition.Value.OriginalFileName!).AbsolutePath.ReplaceOnce("Z:/", "/");
			// 更新方法名
			frame.Method = originalPosition.Value.OriginalName ?? frame.Method;
			// 更新行号
			frame.LineNumber = originalPosition.Value.OriginalLineNumber + 1;
			// 更新列号
			frame.ColumnNumber = originalPosition.Value.OriginalColumnNumber;
			// 如果源根存在，则更新文件名
			if (!string.IsNullOrEmpty(sourceRoot))
				frame.File = frame.File?.Replace(sourceRoot, "");
		}

		// 返回重新格式化的堆栈跟踪
		return string.Join("\n",
			trace.Frames.Select(stackFrame => {
				var pos = $"{Uri.UnescapeDataString(stackFrame.File!)}:{stackFrame.LineNumber}:{stackFrame.ColumnNumber}";

				return string.IsNullOrEmpty(stackFrame.Method)
					? $"    at {pos}"
					: $"    at {stackFrame.Method}{(stackFrame.Arguments.Length > 0 ? $"({string.Join(", ", stackFrame.Arguments)})" : "")} ({pos})";
			}));
	}
}