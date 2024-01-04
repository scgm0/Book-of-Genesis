using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Godot;
using Jint.Native;
using Engine = Jint.Engine;

namespace 创世记;

public partial class Main {
	public static Engine CurrentEngine { get; private set; }
	public static ModInfo CurrentModInfo;
	private JsObject _currentWorld;
	private JsObject _currentWorldEvent;
	private CancellationTokenSource _tcs = new();

	static private readonly Dictionary<string, ModInfo> ModInfos = new();

	static private void LoadModInfos() {
		ModInfos.Clear();
		using (var userDir = DirAccess.Open(Utils.UserModsPath)) {
			userDir.ListDirBegin();
			var fileName = userDir.GetNext();
			while (fileName is not "" and not "." and not "..") {
				var filePath = Utils.UserModsPath.PathJoin(fileName);
				if (userDir.CurrentIsDir()) {
					var modConfigPath = filePath.PathJoin("config.json");
					if (FileAccess.FileExists(modConfigPath)) {
						try {
							var modInfo = JsonSerializer.Deserialize(FileAccess.GetFileAsString(modConfigPath),
								SourceGenerationContext.Default.ModInfo);
							modInfo.Path = $"/{fileName}/";
							modInfo.IsUser = true;
							ModInfos[modInfo.ModKey] = modInfo;
						} catch (Exception e) {
							GD.PrintErr(modConfigPath, e);
						}
					}
				} else if (fileName.GetExtension() == "modpack" || fileName.GetExtension() == "zip") {
					GD.PrintS(filePath, ProjectSettings.LoadResourcePack(filePath, false));
				}

				fileName = userDir.GetNext();
			}
		}

		using (var resDir = DirAccess.Open(Utils.ResModsPath)) {
			if (resDir == null) return;
			resDir.ListDirBegin();
			var fileName = resDir.GetNext();
			while (fileName is not "" and not "." and not "..") {
				var filePath = Utils.ResModsPath.PathJoin(fileName);
				GD.PrintS(filePath, DirAccess.Open(filePath) == null);
				if (resDir.CurrentIsDir()) {
					var modConfigPath = filePath.PathJoin("config.json");
					if (FileAccess.FileExists(modConfigPath)) {
						try {
							var modInfo = JsonSerializer.Deserialize(FileAccess.GetFileAsString(modConfigPath),
								SourceGenerationContext.Default.ModInfo);
							modInfo.Path = $"/{fileName}/";
							modInfo.IsUser = false;
							modInfo.IsEncrypt =
								FileAccess.FileExists(
									$"{Utils.ResModsPath}{modInfo.Path}/{$"{modInfo.Author}:{modInfo.Name}-{modInfo.Version}".EnBase64()}.isEncrypt");
							ModInfos[modInfo.ModKey] = modInfo;
						} catch (Exception e) {
							GD.PrintErr(modConfigPath, e);
						}
					}
				}

				fileName = resDir.GetNext();
			}
		}
	}

}