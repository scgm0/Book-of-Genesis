using System.Collections.Generic;
using System.Threading;
using Godot;
using SourceMaps;
using Timer = System.Timers.Timer;

namespace 创世记;

public static partial class Utils {
	public static readonly Dictionary<string, WorldInfo> WorldInfos = new();
	public static readonly Dictionary<int, Timer> Timers = new();
	public static readonly List<CanvasTexture> TextureCache = [];
	public static readonly ConfigFile GlobalConfig = new();
	public static readonly SceneTree Tree = (SceneTree)Engine.GetMainLoop();
	public static CancellationTokenSource? Tcs { get; set; }
	public static SourceMapCollection? SourceMapCollection { get; set; }
}