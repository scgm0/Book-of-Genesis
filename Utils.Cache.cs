using System.Collections.Generic;
using Godot;

namespace 创世记;

public static partial class Utils {
	public static readonly Dictionary<string, WorldInfo> WorldInfos = new();
	public static readonly List<AudioPlayer> AudioPlayerCache = [];
	public static readonly List<CanvasTexture> TextureCache = [];
	public static readonly ConfigFile GlobalConfig = new();
	public static readonly SceneTree Tree = (SceneTree)Engine.GetMainLoop();
}