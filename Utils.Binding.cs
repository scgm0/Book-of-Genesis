using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Puerts;

namespace 创世记;

[Configure]
public static partial class Utils {
	[Binding]
	public static IEnumerable<Type> Bindings {
		get => [
			typeof(Task),
			typeof(Log),
			typeof(World),
			typeof(TextType),
			typeof(WorldModuleLoader),
		];
	}
}