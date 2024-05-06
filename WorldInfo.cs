using Godot;

namespace 创世记;

public sealed record WorldInfo {
	public required string Name { get; set; }
	public required string Author { get; set; }
	public required string Main { get; set; }
	public string Icon { get; set; } = "";
	public string Version { get; set; } = "0.0.1";
	public string Description { get; set; } = "";

	public string Path { get; set; } = "";
	public string GlobalPath { get; set; } = "";
	public bool IsEncrypt { get; set; }
	public ConfigFile Config { get; set; } = new();
	public string WorldKey { get => $"{Author}_{Name}_{Version}"; }
	public ulong WorldModifiedTime { get; set; }

	public string JsonString {
		get =>
			$"{{\"name\": \"{Name}\", \"author\": \"{Author}\", \"main\": \"{Main}\", \"icon\": \"{Icon}\", \"version\": \"{Version}\", \"description\": \"{Description}\", \"is_encrypt\": {(IsEncrypt ? "true" : "false")}}}";
	}
}