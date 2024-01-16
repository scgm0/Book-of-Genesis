#nullable enable
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
	private readonly bool _restrictToBasePath;
	private readonly bool _isRes;
	private readonly WorldInfo? _worldInfo;

	private CustomModuleLoader(string basePath, bool restrictToBasePath = true) {
		if (string.IsNullOrWhiteSpace(basePath)) {
			Main.Log("值不能为空或空格", nameof(basePath));
		}

		_restrictToBasePath = restrictToBasePath;

		if (!Uri.TryCreate(basePath, UriKind.Absolute, out var temp)) {
			if (!Path.IsPathRooted(basePath)) {
				Main.Log("路径必须是 root 的", nameof(basePath));
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

	public CustomModuleLoader(WorldInfo worldInfo) : this(worldInfo.Path) {
		_worldInfo = worldInfo;
		_isRes = worldInfo.GlobalPath.StartsWith("res://");
	}

	public ResolvedSpecifier Resolve(string? referencingModuleLocation, ModuleRequest moduleRequest) {
		var specifier = moduleRequest.Specifier;
		if (string.IsNullOrEmpty(specifier)) {
			Main.Log("模块说明符无效", specifier, referencingModuleLocation);
			return default!;
		}

		Uri resolved;
		if (Uri.TryCreate(specifier, UriKind.Absolute, out var uri)) {
			resolved = uri;
		} else if (IsRelative(specifier)) {
			resolved = new Uri(referencingModuleLocation != null ? new Uri(_basePath, referencingModuleLocation) : _basePath,
				specifier);
		} else if (specifier[0] == '#') {
			Main.Log($"不支持 PACKAGE_IMPORTS_RESOLVE: '{specifier}'");
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
				Main.Log("模块说明符无效",
					specifier,
					referencingModuleLocation);
				return default!;
			}

			if (!Path.HasExtension(resolved.LocalPath)) {
				Main.Log("不支持的目录导入",
					specifier,
					referencingModuleLocation);
				return default!;
			}
		}

		if (!_restrictToBasePath || _basePath.IsBaseOf(resolved))
			return new ResolvedSpecifier(
				moduleRequest,
				Uri.UnescapeDataString(resolved.AbsolutePath.ReplaceOnce("Z:/", "/")),
				resolved,
				SpecifierType.RelativeOrAbsolute
			);
		Main.Log("未经授权的模块路径", specifier, referencingModuleLocation);
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
			Main.Log(
				$"默认模块加载器只能解析文件。您可以直接定义模块以允许使用 {nameof(Engine)}.{nameof(Engine.Modules.Add)}() 导入。尝试解析：“{resolved.ModuleRequest.Specifier}”。");
			return;
		}

		if (resolved.Uri == null) {
			Main.Log(
				$"“{resolved.Type}”类型的模块“{specifier}”没有解析的 URI。");
		}

		Debug.Assert(resolved.Uri != null, "resolved.Uri != null");
		var fileName =
			$"{(_isRes ? Utils.ResWorldsPath : Utils.UserWorldsPath)}{resolved.Key}";

		if (!FileAccess.FileExists(fileName)) {
			Main.Log("找不到模块: ", specifier);
			return;
		}

		code = FileAccess.GetFileAsString(fileName);

		if (fileName.GetExtension() == "ts") {
			var cachePath = Utils.TsGenPath.PathJoin(resolved.Key.ReplaceOnce(_worldInfo!.Path, $"/{_worldInfo.WorldKey}/"))
				.SimplifyPath();
			if (FileAccess.FileExists($"{cachePath}.meta") &&
				FileAccess.FileExists($"{cachePath}.js")) {
				var tsSha256 = FileAccess.GetSha256(fileName);
				var tsMeta = JsonSerializer.Deserialize(
					FileAccess.GetFileAsString($"{cachePath}.meta"),
					SourceGenerationContext.Default.TsMeta);
				var jsSha256 = FileAccess.GetSha256($"{cachePath}.js");
				if (tsSha256 == tsMeta.TsSha256 && jsSha256 == tsMeta.JsSha256) {
					code = FileAccess.GetFileAsString($"{cachePath}.js");
				} else {
					TsGen(ref code, fileName, resolved);
				}
			} else {
				TsGen(ref code, fileName, resolved);
			}
		}

		RegisterSourceMap(ref code, resolved);
	}

	private void TsGen(ref string code, string fileName, ResolvedSpecifier resolved) {
		if (string.IsNullOrEmpty(code)) return;
		var cachePath = Utils.TsGenPath.PathJoin(resolved.Key.ReplaceOnce(_worldInfo!.Path, $"/{_worldInfo.WorldKey}/"))
			.SimplifyPath();
		DirAccess.MakeDirRecursiveAbsolute($"{cachePath}".GetBaseDir());
		var tsSha256 = FileAccess.GetSha256(fileName);
		var res = TsTransform.Compile(FileAccess.GetFileAsString(fileName), resolved.Key);
		code = res["outputText"].AsString();
		using var jsFile =
			FileAccess.Open($"{cachePath}.js", FileAccess.ModeFlags.Write);
		jsFile.StoreString(code);
		jsFile.Flush();
		var jsSha256 = FileAccess.GetSha256($"{cachePath}.js");
		using var tsMetaFile = FileAccess.Open($"{cachePath}.meta",
			FileAccess.ModeFlags.Write);
		tsMetaFile.StoreString($"{{\"ts_sha256\":\"{tsSha256}\",\"js_sha256\":\"{jsSha256}\"}}");
		tsMetaFile.Flush();
	}

	private void RegisterSourceMap(ref string code, ResolvedSpecifier resolved) {
		if (!Utils.SourceMapPathRegex().IsMatch(code)) return;
		var sourceMappingUrl = Utils.SourceMapPathRegex().Match(code).Value;
		if (sourceMappingUrl.StartsWith("data:application/json;base64,")) {
			Utils.SourceMapCollection.Register(resolved.Key,
				SourceMapParser.Parse(sourceMappingUrl.Replace("data:application/json;base64,", "").UnEnBase64()));
		} else {
			var sourceFile =
				$"{(_isRes ? Utils.ResWorldsPath : Utils.UserWorldsPath)}{new Uri(resolved.Uri!, sourceMappingUrl).AbsolutePath.ReplaceOnce("Z:/", "/")}";
			if (!FileAccess.FileExists(sourceFile)) return;
			var sourceMap = SourceMapParser.Parse(FileAccess.GetFileAsString(sourceFile));
			Utils.SourceMapCollection.Register(resolved.Key, sourceMap);
		}
	}


	static private bool IsRelative(in string specifier) { return specifier.StartsWith('.') || specifier.StartsWith('/'); }

}