using System;
using System.Text.Json;
using Godot;
using Jint.Native;
using Engine = Jint.Engine;

namespace 创世记;

public sealed partial class Main {

	static private JsObject? _currentWorld;
	static private JsObject? _currentWorldEvent;
	public static Engine? CurrentEngine { get; private set; }
	public static WorldInfo? CurrentWorldInfo { get; private set; }

	public static void LoadWorldInfos(string worldsPath, bool loadPackage = false) {
		worldsPath = worldsPath.SimplifyPath();
		using var dir = DirAccess.Open(worldsPath);
		if (dir == null) return;
		dir.ListDirBegin();
		var fileName = dir.GetNext();
		while (fileName is not "" and not "." and not "..") {
			var filePath = worldsPath.PathJoin(fileName);
			if (dir.CurrentIsDir()) {
				DeserializeWorldInfo(filePath.PathJoin("config.json"), fileName, worldsPath, loadPackage);
			} else if (loadPackage &&
				(fileName.GetExtension() == Utils.EncryptionWorldExtension || fileName.GetExtension() == "zip")) {
				Log.Debug(fileName, ProjectSettings.LoadResourcePack(filePath).ToString());
			}

			fileName = dir.GetNext();
		}
	}

	static private void DeserializeWorldInfo(
		string worldConfigPath,
		string fileName,
		string worldsPath,
		bool loadPackage) {
		if (!FileAccess.FileExists(worldConfigPath)) return;
		try {
			var worldInfo = JsonSerializer.Deserialize(FileAccess.GetFileAsString(worldConfigPath),
				SourceGenerationContext.Default.WorldInfo);
			worldInfo!.Path = $"/{fileName}/";
			worldInfo.GlobalPath = worldsPath.PathJoin(fileName).SimplifyPath();
			worldInfo.IsEncrypt = !loadPackage && FileAccess.FileExists(
				$"{worldInfo.GlobalPath}/{$"{worldInfo.Author}:{worldInfo.Name}-{worldInfo.Version}".EnBase64()}.isEncrypt");
			Utils.WorldInfos[worldInfo.WorldKey] = worldInfo;
		} catch (Exception) {
			// ignored
		}
	}
}