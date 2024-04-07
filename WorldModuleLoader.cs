using System.Text.Json;
using Godot;
using Puerts;
using SourceMaps;

namespace 创世记;

public class WorldModuleLoader(WorldInfo? worldInfo) : ILoader, IResolvableLoader, IBuiltinLoadedListener {
	private readonly bool _isRes = worldInfo?.GlobalPath.StartsWith("res://") ?? false;
	private bool _isLoaded;
	private readonly GodotDefaultLoader _defaultLoader = new();
	private readonly SourceMapCollection _sourceMapCollection = new();

	public World? World { get; set; }
	public WorldInfo? WorldInfo { get; } = worldInfo;

	public WorldModuleLoader(World? world) : this(Main.CurrentWorldInfo) {
		World = world;
	}

	public WorldModuleLoader() : this(worldInfo: null) { }

	public string Resolve(string specifier, string referrer) {
		if (!_isLoaded || Utils.Polyfill.ContainsKey(specifier)) return specifier;

		if (specifier.StartsWith("创世记:")) return specifier.ReplaceOnce("创世记:", "");

		var fullPath = specifier;

		if (_isLoaded && specifier.IsRelativePath()) {
			fullPath = referrer.GetBaseDir().PathJoin(specifier);
		}

		return fullPath.SimplifyPath();
	}

	public bool FileExists(string filePath) {
		return true;
	}

	public string? ReadFile(string filePath, out string debugPath) {
		debugPath = string.Empty;
		string? code;
		if (!_isLoaded && _defaultLoader.FileExists(filePath)) {
			code = _defaultLoader.ReadFile(filePath, out debugPath);
			return code;
		}

		if (Utils.Polyfill.TryGetValue(filePath, out code)) {
			debugPath = $"创世记:{filePath}";
			return code;
		}

		if (WorldInfo is null) return code;

		debugPath = WorldInfo.Path.PathJoin(filePath).SimplifyPath();
		var fullPath = WorldInfo.GlobalPath.PathJoin(filePath).SimplifyPath();

		if (!FileAccess.FileExists(fullPath)) return code;

		code = fullPath.EndsWith(".ts") ? ReadTs2Js(fullPath, debugPath) : FileAccess.GetFileAsString(fullPath);
		RegisterSourceMap(in code, debugPath);
		return code;
	}

	public void OnBuiltinLoaded(JsEnv env) {
		JsEnv.ClearAllModuleCaches();
		_isLoaded = true;
		if (WorldInfo is null) return;
		env.ExecuteModule("创世记:world_init");
		env.ExecuteModule("创世记:events");
		env.ExecuteModule("创世记:console");
	}

	public string GetSourceMapStack(string stack) {
		return StackTraceParser.ReTrace(_sourceMapCollection, stack);
	}

	private string ReadTs2Js(string fullPath, string filePath) {
		string code;
		var cachePath = Utils.TsGenPath.PathJoin(filePath.ReplaceOnce(WorldInfo!.Path, $"/{WorldInfo.WorldKey}/")).SimplifyPath();
		var jsPath = $"{cachePath}{(WorldInfo.IsEncrypt ? ".encrypt" : "")}.js";
		var metaPath = $"{cachePath}{(WorldInfo.IsEncrypt ? ".encrypt" : "")}.meta";
		if (FileAccess.FileExists(jsPath) &&
			FileAccess.FileExists(metaPath)) {
			var tsSha256 = FileAccess.GetSha256(fullPath);
			using var metaFile = WorldInfo.IsEncrypt
				? FileAccess.OpenEncryptedWithPass(metaPath,
					FileAccess.ModeFlags.Read,
					$"{Utils.ScriptAes256EncryptionKey}_{WorldInfo.WorldKey}")
				: FileAccess.Open(metaPath, FileAccess.ModeFlags.Read);
			var tsMetaJson = metaFile.GetAsText();

			var tsMeta = JsonSerializer.Deserialize(
				tsMetaJson,
				SourceGenerationContext.Default.TsMeta);
			var jsSha256 = FileAccess.GetSha256(jsPath);
			if (jsSha256 == tsMeta.JsSha256 && tsSha256 == tsMeta.TsSha256) {
				using var jsFile = WorldInfo.IsEncrypt
					? FileAccess.OpenEncryptedWithPass(jsPath,
						FileAccess.ModeFlags.Read,
						$"{Utils.ScriptAes256EncryptionKey}_{WorldInfo.WorldKey}")
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
		var jsPath = $"{cachePath}{(WorldInfo!.IsEncrypt ? ".encrypt" : "")}.js";
		var metaPath = $"{cachePath}{(WorldInfo.IsEncrypt ? ".encrypt" : "")}.meta";
		DirAccess.MakeDirRecursiveAbsolute($"{cachePath}".GetBaseDir());
		var tsSha256 = FileAccess.GetSha256(fullPath);
		var res = TsTransform.Compile(FileAccess.GetFileAsString(fullPath), filePath);
		code = res.Get<string>("outputText");
		var jsFile =
			WorldInfo.IsEncrypt
				? FileAccess.OpenEncryptedWithPass(jsPath,
					FileAccess.ModeFlags.Write,
					$"{Utils.ScriptAes256EncryptionKey}_{WorldInfo.WorldKey}")
				: FileAccess.Open(jsPath, FileAccess.ModeFlags.Write);
		jsFile.StoreString(code);
		jsFile.Dispose();
		var jsSha256 = FileAccess.GetSha256(jsPath);
		var tsMetaFile =
			WorldInfo.IsEncrypt
				? FileAccess.OpenEncryptedWithPass(metaPath,
					FileAccess.ModeFlags.Write,
					$"{Utils.ScriptAes256EncryptionKey}_{WorldInfo.WorldKey}")
				: FileAccess.Open(metaPath, FileAccess.ModeFlags.Write);
		tsMetaFile.StoreString($"{{\"ts_sha256\":\"{tsSha256}\",\"js_sha256\":\"{jsSha256}\"}}");
		tsMetaFile.Dispose();
	}

	private void RegisterSourceMap(in string code, string path) {
		if (!Utils.SourceMapPathRegex().IsMatch(code)) return;
		var sourceMappingUrl = Utils.SourceMapPathRegex().Match(code).Value;
		if (sourceMappingUrl.StartsWith("data:application/json;base64,")) {
			_sourceMapCollection.Register(path,
				SourceMapParser.Parse(sourceMappingUrl.Replace("data:application/json;base64,", "").UnEnBase64()));
		} else {
			var sourceFile =
				$"{(_isRes ? Utils.ResWorldsPath : Utils.UserWorldsPath)}{path.PathJoin(sourceMappingUrl)}".SimplifyPath();
			if (!FileAccess.FileExists(sourceFile)) return;
			var sourceMap = SourceMapParser.Parse(FileAccess.GetFileAsString(sourceFile));
			_sourceMapCollection.Register(path, sourceMap);
		}
	}
}