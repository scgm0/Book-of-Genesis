using System;
using System.Text.Json;
using Godot;

namespace 创世记;

public sealed partial class Main {
	public static WorldInfo? CurrentWorldInfo { get; set; }

	public static void LoadWorldInfos(string worldsPath, bool encrypt = false) {
		worldsPath = worldsPath.SimplifyPath();
		using var dir = DirAccess.Open(worldsPath);
		if (dir == null) return;
		dir.ListDirBegin();
		var fileName = dir.GetNext();
		while (fileName is not "" and not "." and not "..") {
			var filePath = worldsPath.PathJoin(fileName);
			if (dir.CurrentIsDir()) {
				DeserializeWorldInfo(filePath.PathJoin("config.json"), fileName, worldsPath, encrypt);
			}

			fileName = dir.GetNext();
		}
	}

	static private void DeserializeWorldInfo(
		string worldConfigPath,
		string fileName,
		string worldsPath,
		bool encrypt) {
		var configStr = FileAccess.GetFileAsString(worldConfigPath);
		if (configStr.Length <= 1) return;
		try {
			var worldInfo = JsonSerializer.Deserialize(configStr,
				SourceGenerationContext.Default.WorldInfo);
			if (worldInfo == null) return;

			worldInfo.Path = $"/{fileName}/";
			worldInfo.GlobalPath = worldsPath.PathJoin(fileName).SimplifyPath();
			worldInfo.IsEncrypt = encrypt && FileAccess.FileExists(
				$"{worldInfo.GlobalPath}/{worldInfo.WorldKey.EnBase64()}.isEncrypt");

			if (!worldInfo.IsEncrypt) {
				worldInfo.WorldModifiedTime = FileAccess.GetModifiedTime(worldInfo.GlobalPath);
			} else {
				using var encryptFile = FileAccess.Open($"{worldInfo.GlobalPath}/{worldInfo.WorldKey.EnBase64()}.isEncrypt", FileAccess.ModeFlags.Read);
				worldInfo.WorldModifiedTime = encryptFile.Get64();
			}

			Utils.WorldInfos[worldInfo.WorldKey] = worldInfo;
		} catch (Exception e) {
			Log.Debug(e.ToString());
		}
	}
}