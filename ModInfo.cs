using Godot;

namespace 创世记;

public class ModInfo {
	public string Name { get; init; }
	public string Author { get; init; }
	public string Main { get; init; }
	public string Icon { get; init; } = "";
	public string Version { get; init; } = "0.0.1";
	public string Description { get; init; } = "";
	public string Path { get; set; } = "";

	public bool IsUser { get; set; } = true;
	public bool IsEncrypt { get; set; }

	public ConfigFile Config { get; init; } = new();

	public string ModKey {
		get => $"{Author}_{Name}_{Version}";
	}

	public string JsonString {
		get =>
			$"{{\"name\": \"{Name}\", \"author\": \"{Author}\", \"main\": \"{Main}\", \"icon\": \"{Icon}\", \"version\": \"{Version}\", \"description\": \"{Description}\", \"is_encrypt\": {(IsEncrypt ? "true" : "false")}}}";
	}
}