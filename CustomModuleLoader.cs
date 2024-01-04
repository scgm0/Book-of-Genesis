#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using Godot;
using Jint;
using Jint.Runtime.Modules;
using SourceMaps;
using Engine = Jint.Engine;
using FileAccess = Godot.FileAccess;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace 创世记;

public class CustomModuleLoader : ModuleLoader {

	private readonly Uri _basePath;
	private readonly bool _restrictToBasePath;
	private readonly bool _inUser;
	private readonly ModInfo? _modInfo;

	private CustomModuleLoader(string basePath, bool inUser, bool restrictToBasePath = true) {
		if (string.IsNullOrWhiteSpace(basePath)) {
			GD.PrintErr("值不能为空或空格", nameof(basePath));
		}

		_inUser = inUser;
		_restrictToBasePath = restrictToBasePath;

		if (!Uri.TryCreate(basePath, UriKind.Absolute, out var temp)) {
			if (!Path.IsPathRooted(basePath)) {
				GD.PrintErr("路径必须是 root 的", nameof(basePath));
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

	public CustomModuleLoader(ModInfo modInfo) : this(modInfo.Path, modInfo.IsUser) {
		_modInfo = modInfo;
	}

	public override ResolvedSpecifier Resolve(string? referencingModuleLocation, ModuleRequest moduleRequest) {
		var specifier = moduleRequest.Specifier;
		if (string.IsNullOrEmpty(specifier)) {
			GD.PrintErr("模块说明符无效", specifier, referencingModuleLocation);
			return default!;
		}

		Uri resolved;
		if (Uri.TryCreate(specifier, UriKind.Absolute, out var uri)) {
			resolved = uri;
		} else if (IsRelative(specifier)) {
			resolved = new Uri(referencingModuleLocation != null ? new Uri(_basePath, referencingModuleLocation) : _basePath,
				specifier);
		} else if (specifier[0] == '#') {
			GD.PrintErr($"不支持 PACKAGE_IMPORTS_RESOLVE: '{specifier}'");
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
				GD.PrintErr("模块说明符无效",
					specifier,
					referencingModuleLocation);
				return default!;
			}

			if (!Path.HasExtension(resolved.LocalPath)) {
				GD.PrintErr("不支持的目录导入",
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
		GD.PrintErr("未经授权的模块路径", specifier, referencingModuleLocation);
		return default!;
	}

	override protected string LoadModuleContents(Engine engine, ResolvedSpecifier resolved) {
		var specifier = resolved.ModuleRequest.Specifier;
		if (resolved.Type != SpecifierType.RelativeOrAbsolute) {
			GD.PrintErr(
				$"默认模块加载器只能解析文件。您可以直接定义模块以允许使用 {nameof(Engine)}.{nameof(Engine.AddModule)}() 导入。尝试解析：“{resolved.ModuleRequest.Specifier}”。");
			return default!;
		}

		if (resolved.Uri == null) {
			GD.PrintErr(
				$"“{resolved.Type}”类型的模块“{specifier}”没有解析的 URI。");
		}

		Debug.Assert(resolved.Uri != null, "resolved.Uri != null");
		var fileName =
			$"{(_inUser ? Utils.UserModsPath : Utils.ResModsPath)}{resolved.Key}";

		if (!FileAccess.FileExists(fileName)) {
			GD.PrintErr("找不到模块: ", specifier);
			GD.PrintErr(fileName);
			return default!;
		}

		var code = FileAccess.GetFileAsString(fileName);

		if (fileName.GetExtension() == "ts") {
			if (FileAccess.FileExists($"{Utils.TsGenPath}/{_modInfo?.ModKey}{resolved.Key}.meta") &&
				FileAccess.FileExists($"{Utils.TsGenPath}/{_modInfo?.ModKey}{resolved.Key}.js")) {
				var tsSha256 = FileAccess.GetSha256(fileName);
				var tsMeta = JsonSerializer.Deserialize<TsMeta>(
					FileAccess.GetFileAsString($"{Utils.TsGenPath}/{_modInfo?.ModKey}{resolved.Key}.meta"),
					Utils.JsonSerializerOptions);
				var jsSha256 = FileAccess.GetSha256($"{Utils.TsGenPath}/{_modInfo?.ModKey}{resolved.Key}.js");
				if (tsSha256 == tsMeta.TsSha256 && jsSha256 == tsMeta.JsSha256) {
					code = FileAccess.GetFileAsString($"{Utils.TsGenPath}/{_modInfo?.ModKey}{resolved.Key}.js");
				} else {
					TsGen(ref code, fileName, resolved);
				}
			} else {
				TsGen(ref code, fileName, resolved);
			}
		}

		RegisterSourceMap(ref code, resolved);
		return code;
	}


	/*
	public ModuleRecord LoadModule(Engine engine, ResolvedSpecifier resolved){
		var specifier = resolved.ModuleRequest.Specifier;
		if (resolved.Type != SpecifierType.RelativeOrAbsolute) {
			GD.PrintErr(
				$"默认模块加载器只能解析文件。您可以直接定义模块以允许使用 {nameof(Engine)}.{nameof(Engine.AddModule)}() 导入。尝试解析：“{resolved.ModuleRequest.Specifier}”。");
			return default!;
		}

		if (resolved.Uri == null) {
			GD.PrintErr(
				$"“{resolved.Type}”类型的模块“{specifier}”没有解析的 URI。");
		}

		Debug.Assert(resolved.Uri != null, "resolved.Uri != null");
		var fileName =
			$"{(_inUser ? Utils.UserModsPath : Utils.ResModsPath)}{resolved.Key}";

		if (!FileAccess.FileExists(fileName)) {
			GD.PrintErr("找不到模块: ", specifier);
			GD.PrintErr(fileName);
			return default!;
		}

		var code = FileAccess.GetFileAsString(fileName);
		if (fileName.GetExtension() == "js") {
			RegisterSourceMap(ref code, resolved);
		} else if (fileName.GetExtension() == "ts") {
			if (FileAccess.FileExists($"{Utils.TsGenPath}/{_modInfo?.ModKey}{resolved.Key}.meta") &&
				FileAccess.FileExists($"{Utils.TsGenPath}/{_modInfo?.ModKey}{resolved.Key}.js")) {
				var tsSha256 = FileAccess.GetSha256(fileName);
				var tsMeta = JsonSerializer.Deserialize<TsMeta>(
					FileAccess.GetFileAsString($"{Utils.TsGenPath}/{_modInfo?.ModKey}{resolved.Key}.meta"),
					Utils.JsonSerializerOptions);
				var jsSha256 = FileAccess.GetSha256($"{Utils.TsGenPath}/{_modInfo?.ModKey}{resolved.Key}.js");
				if (tsSha256 == tsMeta?.TsSha256 && jsSha256 == tsMeta.JsSha256) {
					code = FileAccess.GetFileAsString($"{Utils.TsGenPath}/{_modInfo?.ModKey}{resolved.Key}.js");
					RegisterSourceMap(ref code, resolved);
				} else {
					TsGen(ref code, fileName, resolved);
				}
			} else {
				TsGen(ref code, fileName, resolved);
			}
		}

		var source = Uri.UnescapeDataString(resolved.Key);
		Module module;
		try {
			module = new JavaScriptParser().ParseModule(code, source);
		} catch (ParserException ex) {
			throw new JavaScriptException($"{ex.Error}").SetJavaScriptCallstack(engine,
				new Location().WithSource(ex.SourceLocation!).WithPosition(Position.From(ex.LineNumber, ex.Column - 1),
					Position.From(ex.LineNumber, ex.Column + 1)));
		} catch (Exception) {
			throw new JavaScriptException($"无法加载模块{source}").SetJavaScriptCallstack(engine, default);
		}

		Debug.Assert(module != null, nameof(module) + " != null");
		return module;
	}
	*/

	private void TsGen(ref string code, string fileName, ResolvedSpecifier resolved) {
		if (string.IsNullOrEmpty(code)) return;
		DirAccess.MakeDirRecursiveAbsolute($"{Utils.TsGenPath}/{_modInfo?.ModKey}{resolved.Key}".GetBaseDir());
		var tsSha256 = FileAccess.GetSha256(fileName);
		var res = Utils.TsTransform.Compile(FileAccess.GetFileAsString(fileName), resolved.Key);
		code = res["outputText"].AsString();
		using var jsFile =
			FileAccess.Open($"{Utils.TsGenPath}/{_modInfo?.ModKey}{resolved.Key}.js", FileAccess.ModeFlags.Write);
		jsFile.StoreString(code);
		jsFile.Flush();
		var jsSha256 = FileAccess.GetSha256($"{Utils.TsGenPath}/{_modInfo?.ModKey}{resolved.Key}.js");
		using var tsMetaFile = FileAccess.Open($"{Utils.TsGenPath}/{_modInfo?.ModKey}{resolved.Key}.meta",
			FileAccess.ModeFlags.Write);
		tsMetaFile.StoreString($"{{\"ts_sha256\":\"{tsSha256}\",\"js_sha256\":\"{jsSha256}\"}}");
		tsMetaFile.Flush();
	}

	private void RegisterSourceMap(ref string code, ResolvedSpecifier resolved) {
		if (!Utils.SourceMapPathRegex().IsMatch(code)) return;
		var sourceMappingUrl = Utils.SourceMapPathRegex().Match(code).Value;
		if (sourceMappingUrl.StartsWith("data:application/json;base64,")) {
			Utils.SourceMapCollection.Register(resolved.Key,
				SourceMapParser.Parse(sourceMappingUrl.ReplaceOnce("data:application/json;base64,", "").UnEnBase64()));
		} else {
			var sourceFile =
				$"{(_inUser ? Utils.UserModsPath : Utils.ResModsPath)}{new Uri(resolved.Uri!, sourceMappingUrl).AbsolutePath.ReplaceOnce("Z:/", "/")}";
			if (!FileAccess.FileExists(sourceFile)) return;
			var sourceMap = SourceMapParser.Parse(FileAccess.GetFileAsString(sourceFile));
			Utils.SourceMapCollection.Register(resolved.Key, sourceMap);
		}
	}


	static private bool IsRelative(in string specifier) {
		return specifier.StartsWith('.') || specifier.StartsWith('/');
	}

}