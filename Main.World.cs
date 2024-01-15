using System;
using System.Text.Json;
using Godot;
using Jint.Native;
using Engine = Jint.Engine;

namespace 创世记;

public sealed partial class Main {

	static private JsObject _currentWorld;
	static private JsObject _currentWorldEvent;
	public static Engine CurrentEngine { get; private set; }
	public static WorldInfo CurrentWorldInfo { get; private set; }

	static private void LoadWorldInfos(string worldsPath, bool loadPackage = false) {
		using var dir = DirAccess.Open(worldsPath.SimplifyPath());
		if(dir == null) return;
		dir.ListDirBegin();
		var fileName = dir.GetNext();
		while (fileName is not "" and not "." and not "..") {
			var filePath = worldsPath.PathJoin(fileName);
			if (dir.CurrentIsDir()) {
				var worldConfigPath = filePath.PathJoin("config.json");
				if (FileAccess.FileExists(worldConfigPath)) {
					try {
						var worldInfo = JsonSerializer.Deserialize(FileAccess.GetFileAsString(worldConfigPath),
							SourceGenerationContext.Default.WorldInfo);
						worldInfo.Path = $"/{fileName}/";
						worldInfo.GlobalPath = worldsPath.PathJoin(fileName).SimplifyPath();
						Utils.WorldInfos[worldInfo.WorldKey] = worldInfo;
						if (!loadPackage) {
							worldInfo.IsEncrypt =
								FileAccess.FileExists(
									$"{worldInfo.GlobalPath}/{$"{worldInfo.Author}:{worldInfo.Name}-{worldInfo.Version}".EnBase64()}.isEncrypt");
						}
					} catch (Exception e) {
						if (loadPackage) {
							Log(ProjectSettings.GlobalizePath(worldConfigPath), e);
						}
					}
				}
			} else if (loadPackage && (fileName.GetExtension() == "modpack" || fileName.GetExtension() == "zip")) {
				Log(fileName, ProjectSettings.LoadResourcePack(filePath));
			}

			fileName = dir.GetNext();
		}
	}
}