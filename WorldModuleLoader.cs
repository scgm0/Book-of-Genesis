using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
using Puerts;
using SourceMaps;

namespace 创世记;

public class WorldModuleLoader(WorldInfo? worldInfo) : ILoader, IResolvableLoader, IBuiltinLoadedListener {
	private readonly bool _isRes = worldInfo?.GlobalPath.StartsWith("res://") ?? false;
	private bool _isLoaded;
	private readonly GodotDefaultLoader _defaultLoader = new();

	public string Resolve(string specifier, string referrer) {
		if (!_isLoaded) return specifier;

		if (specifier.StartsWith("创世记:")) return specifier.ReplaceOnce("创世记:", "");

		var fullPath = specifier;

		if (_isLoaded && specifier.IsRelativePath()) {
			fullPath = referrer.GetBaseDir().PathJoin(specifier);
		}

		return fullPath.SimplifyPath();
	}

	public bool FileExists(string filePath) {
		if (!_isLoaded && _defaultLoader.FileExists(filePath)) {
			return true;
		}

		if (Utils.Polyfill.ContainsKey(filePath)) {
			return true;
		}

		if (worldInfo is null) return false;

		var fullPath = worldInfo.GlobalPath.PathJoin(filePath).SimplifyPath();
		return Utils.Polyfill.ContainsKey(filePath) || FileAccess.FileExists(fullPath);
	}

	public string ReadFile(string filePath, out string debugPath) {
		debugPath = string.Empty;
		string code;
		if (!_isLoaded && _defaultLoader.FileExists(filePath)) {
			code = _defaultLoader.ReadFile(filePath, out debugPath);
			return code;
		}

		if (Utils.Polyfill.TryGetValue(filePath, out var value)) {
			debugPath = $"创世记:{filePath}";
			return value;
		}

		if (worldInfo is null) return string.Empty;

		debugPath = worldInfo.Path.PathJoin(filePath).SimplifyPath();
		var fullPath = worldInfo.GlobalPath.PathJoin(filePath).SimplifyPath();
		code = fullPath.EndsWith(".ts") ? ReadTs2Js(fullPath, debugPath) : FileAccess.GetFileAsString(fullPath);
		RegisterSourceMap(in code, debugPath);
		return code;
	}

	public void OnBuiltinLoaded(JsEnv env) {
		_defaultLoader.OnBuiltinLoaded(env);
		_isLoaded = true;
		env.Eval("global.World = {};");
		env.ExecuteModule("创世记:console");
	}

	private string ReadTs2Js(string fullPath, string filePath) {
		string code;
		var cachePath = Utils.TsGenPath.PathJoin(filePath.ReplaceOnce(worldInfo!.Path, $"/{worldInfo.WorldKey}/")).SimplifyPath();
		var jsPath = $"{cachePath}{(worldInfo.IsEncrypt ? ".encrypt" : "")}.js";
		var metaPath = $"{cachePath}{(worldInfo.IsEncrypt ? ".encrypt" : "")}.meta";
		if (FileAccess.FileExists(jsPath) &&
			FileAccess.FileExists(metaPath)) {
			var tsSha256 = FileAccess.GetSha256(fullPath);
			using var metaFile = worldInfo.IsEncrypt
				? FileAccess.OpenEncryptedWithPass(metaPath,
					FileAccess.ModeFlags.Read,
					$"{Utils.ScriptAes256EncryptionKey}_{worldInfo.WorldKey}")
				: FileAccess.Open(metaPath, FileAccess.ModeFlags.Read);
			var tsMetaJson = metaFile.GetAsText();

			var tsMeta = JsonSerializer.Deserialize(
				tsMetaJson,
				SourceGenerationContext.Default.TsMeta);
			var jsSha256 = FileAccess.GetSha256(jsPath);
			if (jsSha256 == tsMeta.JsSha256 && tsSha256 == tsMeta.TsSha256) {
				using var jsFile = worldInfo.IsEncrypt
					? FileAccess.OpenEncryptedWithPass(jsPath,
						FileAccess.ModeFlags.Read,
						$"{Utils.ScriptAes256EncryptionKey}_{worldInfo.WorldKey}")
					: FileAccess.Open(jsPath, FileAccess.ModeFlags.Read);
				code = jsFile.GetAsText();
			} else {
				TsGen(out code, fullPath, filePath, cachePath);
			}
		} else {
			TsGen(out code, fullPath, filePath, cachePath);
		}

		return code;
	}

	private void TsGen(out string code, string fullPath, string filePath, string cachePath) {
		var jsPath = $"{cachePath}{(worldInfo!.IsEncrypt ? ".encrypt" : "")}.js";
		var metaPath = $"{cachePath}{(worldInfo.IsEncrypt ? ".encrypt" : "")}.meta";
		DirAccess.MakeDirRecursiveAbsolute($"{cachePath}".GetBaseDir());
		var tsSha256 = FileAccess.GetSha256(fullPath);
		var res = TsTransform.Compile(FileAccess.GetFileAsString(fullPath), filePath);
		code = res.Get<string>("outputText");
		var jsFile =
			worldInfo.IsEncrypt
				? FileAccess.OpenEncryptedWithPass(jsPath,
					FileAccess.ModeFlags.Write,
					$"{Utils.ScriptAes256EncryptionKey}_{worldInfo.WorldKey}")
				: FileAccess.Open(jsPath, FileAccess.ModeFlags.Write);
		jsFile.StoreString(code);
		jsFile.Dispose();
		var jsSha256 = FileAccess.GetSha256(jsPath);
		var tsMetaFile =
			worldInfo.IsEncrypt
				? FileAccess.OpenEncryptedWithPass(metaPath,
					FileAccess.ModeFlags.Write,
					$"{Utils.ScriptAes256EncryptionKey}_{worldInfo.WorldKey}")
				: FileAccess.Open(metaPath, FileAccess.ModeFlags.Write);
		tsMetaFile.StoreString($"{{\"ts_sha256\":\"{tsSha256}\",\"js_sha256\":\"{jsSha256}\"}}");
		tsMetaFile.Dispose();
	}

	private void RegisterSourceMap(in string code, string path) {
		if (!Utils.SourceMapPathRegex().IsMatch(code)) return;
		var sourceMappingUrl = Utils.SourceMapPathRegex().Match(code).Value;
		if (sourceMappingUrl.StartsWith("data:application/json;base64,")) {
			Utils.SourceMapCollection?.Register(path,
				SourceMapParser.Parse(sourceMappingUrl.Replace("data:application/json;base64,", "").UnEnBase64()));
		} else {
			var sourceFile =
				$"{(_isRes ? Utils.ResWorldsPath : Utils.UserWorldsPath)}{path.PathJoin(sourceMappingUrl)}".SimplifyPath();
			if (!FileAccess.FileExists(sourceFile)) return;
			var sourceMap = SourceMapParser.Parse(FileAccess.GetFileAsString(sourceFile));
			Utils.SourceMapCollection?.Register(path, sourceMap);
		}
	}
}