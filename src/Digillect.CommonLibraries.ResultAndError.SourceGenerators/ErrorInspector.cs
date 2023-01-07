using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Digillect.CommonLibraries.ResultAndError.SourceGenerators;

internal sealed class ErrorInspector
{
	private const string BaseErrorTypeName = "Digillect.Error";

	private readonly INamedTypeSymbol _errorTypeSymbol;
	private readonly INamedTypeSymbol _baseErrorTypeSymbol;

	public ErrorInspector(INamedTypeSymbol errorTypeSymbol, Compilation compilation)
	{
		_errorTypeSymbol = errorTypeSymbol;

		_baseErrorTypeSymbol = compilation.GetTypeByMetadataName(BaseErrorTypeName)!;
	}

	public ErrorModel Inspect()
	{
		return new ErrorModel(
			_errorTypeSymbol.Name,
			_errorTypeSymbol.ContainingNamespace.ToString(),
			FindCases().ToImmutableArray());
	}

	private IEnumerable<ErrorCase> FindCases()
	{
		foreach (var method in _errorTypeSymbol.GetMembers().OfType<IMethodSymbol>().Where(IsValidErrorCaseFactory))
		{
			yield return new ErrorCase(
				method.Name,
				$"{method.Name}Error",
				method.DeclaredAccessibility == Accessibility.Public,
				GetFactoryReturnTypeName(method),
				GetOriginalParametersDeclaration(method),
				method.Parameters.Select(ConstructParameter).ToImmutableArray());
		}
	}

	private static string GetOriginalParametersDeclaration(IMethodSymbol method)
	{
		return ((MethodDeclarationSyntax) method.DeclaringSyntaxReferences[0].GetSyntax()).ParameterList.ToString();
	}

	private string GetFactoryReturnTypeName(IMethodSymbol method)
	{
		return method.ReturnType.Equals(_baseErrorTypeSymbol, SymbolEqualityComparer.Default)
			? BaseErrorTypeName
			: ((INamedTypeSymbol) method.ReturnType).Name;
	}

	private static ErrorCaseParameter ConstructParameter(IParameterSymbol parameter)
	{
		string propertyName = parameter.Name.Length == 1
			? parameter.Name.ToUpper()
			: char.ToUpper(parameter.Name[0]) + parameter.Name[1..];

		return new ErrorCaseParameter(parameter.Name, propertyName, parameter.Type.ToString()!);
	}

	private bool IsValidErrorCaseFactory(IMethodSymbol method)
	{
		var returnType = method.ReturnType;

		if (!returnType.Equals(_errorTypeSymbol, SymbolEqualityComparer.Default) && !returnType.Equals(_baseErrorTypeSymbol, SymbolEqualityComparer.Default))
		{
			return false;
		}

		if (!method.IsStatic || !method.IsPartialDefinition)
		{
			return false;
		}

		if (method.DeclaredAccessibility != Accessibility.Public && method.DeclaredAccessibility != Accessibility.Internal)
		{
			return false;
		}

		return true;
	}
}
