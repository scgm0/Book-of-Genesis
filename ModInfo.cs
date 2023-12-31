#nullable enable
using Godot;

namespace 创世记;

public class ModInfo {
	public required string Name { get; set; }
	public required string Author { get; set; }
	public required string Main { get; set; }
	public string Icon { get; set; } = "";
	public string Version { get; set; } = "0.0.1";
	public string Description { get; set; } = "";
	public string Path { get; set; } = "";

	public bool IsUser { get; set; } = true;
	public bool IsEncrypt { get; set; }

	public ConfigFile Config { get; set; } = new();

	public string ModKey { get => $"{Author}_{Name}_{Version}"; }

	public string JsonString {
		get =>
			$"{{\"name\": \"{Name}\", \"author\": \"{Author}\", \"main\": \"{Main}\", \"icon\": \"{Icon}\", \"version\": \"{Version}\", \"description\": \"{Description}\", \"is_encrypt\": {(IsEncrypt ? "true" : "false")}}}";
	}
}