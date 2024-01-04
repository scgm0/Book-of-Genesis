using System.Text.Json.Serialization;

namespace 创世记;

[JsonSerializable(typeof(ModInfo))]
[JsonSerializable(typeof(TsMeta))]
public partial class SourceGenerationContext: JsonSerializerContext;