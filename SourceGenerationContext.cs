using System.Text.Json.Serialization;

namespace 创世记;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower, AllowTrailingCommas = true)]
[JsonSerializable(typeof(ModInfo))]
[JsonSerializable(typeof(TsMeta))]
sealed partial class SourceGenerationContext : JsonSerializerContext;