using Microsoft.CodeAnalysis;

namespace Digillect.CommonLibraries.ResultAndError.SourceGenerators;

public static class Helpers
{
	public static bool InheritsFrom(INamedTypeSymbol typeToCheck, ITypeSymbol baseType)
	{
		var type = typeToCheck.BaseType;

		while (type is not null)
		{
			if (type.Equals(baseType, SymbolEqualityComparer.Default))
			{
				return true;
			}

			type = type.BaseType;
		}

		return false;
	}
}
