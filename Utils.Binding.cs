using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Puerts;

namespace 创世记;

[Configure]
public static partial class Utils {
	[Binding]
	// ReSharper disable once UnusedMember.Global
	public static IEnumerable<Type> Bindings {
		get => [
			typeof(Array),
			typeof(Task),
			typeof(Log),
			typeof(Utils),
			typeof(World),
			typeof(WorldInfo),
			typeof(WorldModuleLoader),
			typeof(FilterType),
			typeof(TextType),
			typeof(EventType)
		];
	}
}