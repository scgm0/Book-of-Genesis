using System;
using System.Text.Json;
using System.Threading;
using Godot;
using Jint.Native;
using Engine = Jint.Engine;

namespace 创世记;

public sealed partial class Main {

	private JsObject _currentWorld;
	private JsObject _currentWorldEvent;
	private CancellationTokenSource _tcs = new();
	public static Engine CurrentEngine { get; private set; }
	public static WorldInfo CurrentWorldInfo { get; private set; }

	static private void LoadWorldInfos() {
		using (var userDir = DirAccess.Open(Utils.UserWorldsPath)) {
			userDir.ListDirBegin();
			var fileName = userDir.GetNext();
			while (fileName is not "" and not "." and not "..") {
				var filePath = Utils.UserWorldsPath.PathJoin(fileName);
				if (userDir.CurrentIsDir()) {
					var worldConfigPath = filePath.PathJoin("config.json");
					if (FileAccess.FileExists(worldConfigPath)) {
						try {
							var worldInfo = JsonSerializer.Deserialize(FileAccess.GetFileAsString(worldConfigPath),
								SourceGenerationContext.Default.WorldInfo);
							worldInfo.Path = $"/{fileName}/";
							worldInfo.IsUser = true;
							Utils.WorldInfos[worldInfo.WorldKey] = worldInfo;
						} catch (Exception e) {
							Log(worldConfigPath, e);
						}
					}
				} else if (fileName.GetExtension() == "modpack" || fileName.GetExtension() == "zip") {
					Log(fileName, ProjectSettings.LoadResourcePack(filePath));
				}

				fileName = userDir.GetNext();
			}
		}

		using (var resDir = DirAccess.Open(Utils.ResWorldsPath)) {
			if (resDir == null) return;
			resDir.ListDirBegin();
			var fileName = resDir.GetNext();
			while (fileName is not "" and not "." and not "..") {
				var filePath = Utils.ResWorldsPath.PathJoin(fileName);
				if (resDir.CurrentIsDir()) {
					var worldConfigPath = filePath.PathJoin("config.json");
					if (FileAccess.FileExists(worldConfigPath)) {
						try {
							var worldInfo = JsonSerializer.Deserialize(FileAccess.GetFileAsString(worldConfigPath),
								SourceGenerationContext.Default.WorldInfo);
							worldInfo.Path = $"/{fileName}/";
							worldInfo.IsUser = false;
							worldInfo.IsEncrypt =
								FileAccess.FileExists(
									$"{Utils.ResWorldsPath}{worldInfo.Path}/{$"{worldInfo.Author}:{worldInfo.Name}-{worldInfo.Version}".EnBase64()}.isEncrypt");
							Utils.WorldInfos[worldInfo.WorldKey] = worldInfo;
						} catch (Exception) {
							// ignored
						}
					}
				}

				fileName = resDir.GetNext();
			}
		}
	}
}