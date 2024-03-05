using System;
using System.Diagnostics;
using System.IO;
using Esprima;
using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Json;
using Jint.Runtime;
using Jint.Runtime.Modules;
using SourceMaps;
using Engine = Jint.Engine;
using FileAccess = Godot.FileAccess;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace 创世记;

sealed class CustomModuleLoader : IModuleLoader {

	private readonly Uri _basePath;
	private readonly bool _isRes;
	private readonly WorldInfo _worldInfo;

	public CustomModuleLoader(WorldInfo worldInfo) {
		_worldInfo = worldInfo;
		_isRes = worldInfo.GlobalPath.StartsWith("res://");
		var basePath = _worldInfo.Path;
		if (string.IsNullOrWhiteSpace(basePath)) {
			Log.Error("值不能为空或空格", nameof(basePath));
		}

		if (!Uri.TryCreate(basePath, UriKind.Absolute, out var temp)) {
			if (!Path.IsPathRooted(basePath)) {
				Log.Error("路径必须是 root 的", nameof(basePath));
			}

			Debug.Assert(basePath != null, nameof(basePath) + " != null");
			basePath = Path.GetFullPath(basePath);
			_basePath = new Uri(basePath, UriKind.Absolute);
		} else {
			_basePath = temp;
		}

		if (_basePath.AbsolutePath[^1] == '/') return;
		var uriBuilder = new UriBuilder(_basePath);
		uriBuilder.Path += '/';
		_basePath = uriBuilder.Uri;
	}

	public ResolvedSpecifier Resolve(string? referencingModuleLocation, ModuleRequest moduleRequest) {
		var specifier = moduleRequest.Specifier;
		if (string.IsNullOrEmpty(specifier)) {
			Log.Error("模块说明符无效", specifier, referencingModuleLocation ?? string.Empty);
			return default!;
		}

		Uri resolved;
		if (Uri.TryCreate(specifier, UriKind.Absolute, out var uri)) {
			resolved = uri;
		} else if (IsRelative(specifier)) {
			resolved = new Uri(referencingModuleLocation != null
					? new Uri(_basePath, referencingModuleLocation)
					: _basePath,
				specifier);
		} else if (specifier[0] == '#') {
			Log.Error($"不支持 PACKAGE_IMPORTS_RESOLVE: '{specifier}'");
			return default!;
		} else {
			return new ResolvedSpecifier(
				moduleRequest,
				specifier,
				null,
				SpecifierType.Bare
			);
		}

		if (resolved.IsFile) {
			if (resolved.UserEscaped) {
				Log.Error("模块说明符无效",
					specifier,
					referencingModuleLocation ?? string.Empty);
				return default!;
			}

			if (!Path.HasExtension(resolved.LocalPath)) {
				Log.Error("不支持的目录导入",
					specifier,
					referencingModuleLocation ?? string.Empty);
				return default!;
			}
		}

		if (_basePath.IsBaseOf(resolved))
			return new ResolvedSpecifier(
				moduleRequest,
				Uri.UnescapeDataString(
#if GODOT_WINDOWS
					Utils.DriveLetterRegex().Replace(resolved.AbsolutePath, "/", 1)
#else
					resolved.AbsolutePath
#endif
				),
				resolved,
				SpecifierType.RelativeOrAbsolute
			);
		Log.Error("未经授权的模块路径", specifier, referencingModuleLocation ?? string.Empty);
		return default!;
	}

	public Module LoadModule(Engine engine, ResolvedSpecifier resolved) {
		string code;
		var source = resolved.Key;

		try {
			LoadModuleContents(resolved, out code);
		} catch (Exception e) {
			throw new JavaScriptException($"无法加载模块: {source}\n{e}");
		}

		var isJson = resolved.ModuleRequest.IsJsonModule();
		Module moduleRecord;
		if (isJson) {
			JsValue module;
			try {
				module = new JsonParser(engine).Parse(code);
			} catch (Exception e) {
				throw new JavaScriptException($"无法加载模块: {source}\n{e}");
			}

			moduleRecord = ModuleFactory.BuildJsonModule(engine, module, source);
		} else {
			Esprima.Ast.Module module;
			try {
				module = new JavaScriptParser().ParseModule(code, source);
			} catch (ParserException ex) {
				throw new JavaScriptException($"加载模块时出错: {source}\n{ex.Error}");
			} catch (Exception e) {
				throw new JavaScriptException($"无法加载模块: {source}\n{e}");
			}

			moduleRecord = ModuleFactory.BuildSourceTextModule(engine, module);
		}

		return moduleRecord;
	}

	private void LoadModuleContents(ResolvedSpecifier resolved, out string code) {
		code = "";
		var specifier = resolved.ModuleRequest.Specifier;
		if (resolved.Type != SpecifierType.RelativeOrAbsolute) {
			Log.Error(
				$"默认模块加载器只能解析文件。您可以直接定义模块以允许使用 {nameof(Engine)}.{nameof(Engine.Modules.Add)}() 导入。尝试解析：“{resolved.ModuleRequest.Specifier}”。");
			return;
		}

		if (resolved.Uri == null) {
			Log.Error(
				$"“{resolved.Type}”类型的模块“{specifier}”没有解析的 URI。");
		}

		var fileName =
			$"{(_isRes ? Utils.ResWorldsPath : Utils.UserWorldsPath)}{resolved.Key}";

		if (!FileAccess.FileExists(fileName)) {
			Log.Error("找不到模块: ", specifier);
			return;
		}

		code = fileName.GetExtension() == "ts" ? ReadTs2Js(fileName, resolved) : FileAccess.GetFileAsString(fileName);
		RegisterSourceMap(in code, resolved);
	}

	private string ReadTs2Js(string fileName, ResolvedSpecifier resolved) {
		string code;
		var cachePath = Utils.TsGenPath.PathJoin(resolved.Key.ReplaceOnce(_worldInfo.Path, $"/{_worldInfo.WorldKey}/"))
			.SimplifyPath();
		var jsPath = $"{cachePath}{(_worldInfo.IsEncrypt ? ".encrypt" : "")}.js";
		var metaPath = $"{cachePath}{(_worldInfo.IsEncrypt ? ".encrypt" : "")}.meta";
		if (FileAccess.FileExists(jsPath) &&
			FileAccess.FileExists(metaPath)) {
			var tsSha256 = FileAccess.GetSha256(fileName);
			using var metaFile = _worldInfo.IsEncrypt
				? FileAccess.OpenEncryptedWithPass(metaPath,
					FileAccess.ModeFlags.Read,
					$"{Utils.ScriptAes256EncryptionKey}_{_worldInfo.WorldKey}")
				: FileAccess.Open(metaPath, FileAccess.ModeFlags.Read);
			var tsMetaJson = metaFile.GetAsText();

			var tsMeta = JsonSerializer.Deserialize(
				tsMetaJson,
				SourceGenerationContext.Default.TsMeta);
			var jsSha256 = FileAccess.GetSha256(jsPath);
			if (jsSha256 == tsMeta.JsSha256 && tsSha256 == tsMeta.TsSha256) {
				using var jsFile = _worldInfo.IsEncrypt
					? FileAccess.OpenEncryptedWithPass(jsPath,
						FileAccess.ModeFlags.Read,
						$"{Utils.ScriptAes256EncryptionKey}_{_worldInfo.WorldKey}")
					: FileAccess.Open(jsPath, FileAccess.ModeFlags.Read);
				code = jsFile.GetAsText();
			} else {
				TsGen(out code, fileName, resolved);
			}
		} else {
			TsGen(out code, fileName, resolved);
		}

		return code;
	}

	private void TsGen(out string code, string fileName, ResolvedSpecifier resolved) {
		var cachePath = Utils.TsGenPath.PathJoin(resolved.Key.ReplaceOnce(_worldInfo.Path, $"/{_worldInfo.WorldKey}/"))
			.SimplifyPath();
		var jsPath = $"{cachePath}{(_worldInfo.IsEncrypt ? ".encrypt" : "")}.js";
		var metaPath = $"{cachePath}{(_worldInfo.IsEncrypt ? ".encrypt" : "")}.meta";
		DirAccess.MakeDirRecursiveAbsolute($"{cachePath}".GetBaseDir());
		var tsSha256 = FileAccess.GetSha256(fileName);
		var res = TsTransform.Compile(FileAccess.GetFileAsString(fileName), resolved.Key);
		code = res["outputText"].AsString();
		var jsFile =
			_worldInfo.IsEncrypt
				? FileAccess.OpenEncryptedWithPass(jsPath,
					FileAccess.ModeFlags.Write,
					$"{Utils.ScriptAes256EncryptionKey}_{_worldInfo.WorldKey}")
				: FileAccess.Open(jsPath, FileAccess.ModeFlags.Write);
		jsFile.StoreString(code);
		jsFile.Dispose();
		var jsSha256 = FileAccess.GetSha256(jsPath);
		var tsMetaFile =
			_worldInfo.IsEncrypt
				? FileAccess.OpenEncryptedWithPass(metaPath,
					FileAccess.ModeFlags.Write,
					$"{Utils.ScriptAes256EncryptionKey}_{_worldInfo.WorldKey}")
				: FileAccess.Open(metaPath, FileAccess.ModeFlags.Write);
		tsMetaFile.StoreString($"{{\"ts_sha256\":\"{tsSha256}\",\"js_sha256\":\"{jsSha256}\"}}");
		tsMetaFile.Dispose();
	}

	private void RegisterSourceMap(in string code, ResolvedSpecifier resolved) {
		if (!Utils.SourceMapPathRegex().IsMatch(code)) return;
		var sourceMappingUrl = Utils.SourceMapPathRegex().Match(code).Value;
		if (sourceMappingUrl.StartsWith("data:application/json;base64,")) {
			Utils.SourceMapCollection?.Register(resolved.Key,
				SourceMapParser.Parse(sourceMappingUrl.Replace("data:application/json;base64,", "").UnEnBase64()));
		} else {
			var sourceFile =
				$"{(_isRes ? Utils.ResWorldsPath : Utils.UserWorldsPath)}{Utils.DriveLetterRegex().Replace(new Uri(resolved.Uri!, sourceMappingUrl).AbsolutePath, "/", 1)}";
			if (!FileAccess.FileExists(sourceFile)) return;
			var sourceMap = SourceMapParser.Parse(FileAccess.GetFileAsString(sourceFile));
			Utils.SourceMapCollection?.Register(resolved.Key, sourceMap);
		}
	}

	static private bool IsRelative(in string specifier) { return specifier.StartsWith('.') || specifier.StartsWith('/'); }

}