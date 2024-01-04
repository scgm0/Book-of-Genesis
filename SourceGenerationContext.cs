using System.Text.Json.Serialization;

namespace 创世记;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(ModInfo))]
[JsonSerializable(typeof(TsMeta))]
public partial class SourceGenerationContext: JsonSerializerContext {
	
}