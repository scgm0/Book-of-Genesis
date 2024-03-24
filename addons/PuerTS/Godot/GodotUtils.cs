#if TOOLS

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Godot;

// ReSharper disable CheckNamespace

public static class GodotUtils {
	public static class Platforms {
		public const string Windows = "windows";
		public const string MacOS = "macos";
		public const string LinuxBSD = "linuxbsd";
		public const string Android = "android";
		public const string iOS = "ios";
		public const string Web = "web";
	}

	public static readonly Dictionary<string, string> PlatformFeatureMap = new(
		StringComparer.InvariantCultureIgnoreCase
	) {
		["Windows"] = Platforms.Windows,
		["macOS"] = Platforms.MacOS,
		["Linux"] = Platforms.LinuxBSD,
		["Android"] = Platforms.Android,
		["iOS"] = Platforms.iOS,
		["Web"] = Platforms.Web
	};

	public static string GetLibraryPath(string platform, string arch) {
		return platform switch {
			Platforms.LinuxBSD => ProjectSettings.GlobalizePath("res://addons/PuerTS/Plugins/x86_64/libpuerts.so"),
			Platforms.Windows => arch switch {
				"x86_64" => ProjectSettings.GlobalizePath("res://addons/PuerTS/Plugins/x86_64/puerts.dll"),
				"x86_32" => ProjectSettings.GlobalizePath("res://addons/PuerTS/Plugins/x86/puerts.dll"),
				_ => throw new ArgumentOutOfRangeException(nameof(arch), arch, null)
			},
			Platforms.Android => arch switch {
				"arm64" => ProjectSettings.GlobalizePath("res://addons/PuerTS/Plugins/Android/libs/arm64-v8a/libpuerts.so"),
				"armeabi" => ProjectSettings.GlobalizePath(
					"res://addons/PuerTS/Plugins/Android/libs/armeabi-v7a/libpuerts.so"),
				"x86_64" => ProjectSettings.GlobalizePath("res://addons/PuerTS/Plugins/Android/libs/x86_64/libpuerts.so"),
				_ => throw new ArgumentOutOfRangeException(nameof(arch), arch, null)
			},
			_ => throw new ArgumentOutOfRangeException(nameof(arch), arch, null)
		};
	}

	public static void LoadLibrary() {
		NativeLibrary.Load(GetLibraryPath(PlatformFeatureMap[OS.GetName()],
			Engine.GetArchitectureName()));
	}
}

#endif