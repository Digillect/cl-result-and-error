using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Digillect.CommonLibraries.ResultAndError.SourceGenerators;

[Generator]
public sealed class ErrorImplementationGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var models = context.SyntaxProvider.CreateSyntaxProvider(
				predicate: (s, _) => s is ClassDeclarationSyntax,
				transform: TryBuildErrorModel)
			.Collect();

		context.RegisterSourceOutput(models, GenerateSources);
	}

	private static ErrorModel? TryBuildErrorModel(GeneratorSyntaxContext context, CancellationToken cancellationToken)
	{
		var cd = (ClassDeclarationSyntax) context.Node;

		if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, cd, cancellationToken) is INamedTypeSymbol classSymbol)
		{
			var compilation = context.SemanticModel.Compilation;
			var baseErrorSymbol = compilation.GetTypeByMetadataName("Digillect.Error");

			if (baseErrorSymbol is not null
				&& classSymbol is { DeclaredAccessibility: Accessibility.Internal or Accessibility.Public }
				&& cd.Modifiers.Any(kind => kind.IsKind(SyntaxKind.PartialKeyword))
				&& Helpers.InheritsFrom(classSymbol, baseErrorSymbol))
			{
				var model = new ErrorInspector(classSymbol, compilation).Inspect();

				if (model.Cases.Count > 0)
				{
					return model;
				}
			}
		}

		return null;
	}

	private static void GenerateSources(SourceProductionContext context, ImmutableArray<ErrorModel?> models)
	{
		foreach (var model in models)
		{
			if (model is not null)
			{
				(string fileName, string sourceCode) = GenerateSource(model);

				context.AddSource(fileName, SourceText.From(sourceCode, Encoding.UTF8));
			}
		}
	}

	private static (string, string) GenerateSource(ErrorModel model)
	{
		string fileName = model.Namespace is null ? $"global.{model.Name}.cs" : $"{model.Namespace}.{model.Name}.cs";
		var sb = new StringBuilder();

		if (model.Namespace is not null)
		{
			sb.Append("namespace ").Append(model.Namespace).AppendLine(";");
			sb.AppendLine();
		}

		sb.Append("partial class ").AppendLine(model.Name);
		sb.Append('{');

		foreach (var errorCase in model.Cases)
		{
			GenerateCaseFactory(sb, errorCase);
		}

		foreach (var errorCase in model.Cases)
		{
			GenerateCaseType(sb, errorCase, model.Name);
		}

		sb.AppendLine("}");

		return (fileName, sb.ToString());
	}

	private static void GenerateCaseFactory(StringBuilder sb, ErrorCase errorCase)
	{
		sb.AppendLine();

		sb.Append('\t')
			.Append(errorCase.IsPublicFactory ? "public" : "internal")
			.Append(" static partial ")
			.Append(errorCase.FactoryReturnTypeName)
			.Append(' ')
			.Append(errorCase.Name)
			.Append(errorCase.OriginalParametersDeclaration)
			.Append(" => ");

		if (errorCase.Parameters.Count > 0)
		{
			sb.Append("new ").Append(errorCase.TypeName).AppendLine(" {");

			foreach (var parameter in errorCase.Parameters)
			{
				sb.Append("\t\t").Append(parameter.PropertyName).Append(" = ").Append(parameter.Name).AppendLine(",");
			}

			sb.AppendLine("\t};");
		}
		else
		{
			sb.Append(errorCase.TypeName).AppendLine(".Instance;");
		}
	}

	private static void GenerateCaseType(StringBuilder sb, ErrorCase errorCase, string baseTypeName)
	{
		sb.AppendLine();
		sb.Append("\tpublic sealed partial class ").Append(errorCase.TypeName).Append(" : ").AppendLine(baseTypeName);
		sb.AppendLine("\t{");

		if (errorCase.Parameters.Count > 0)
		{
			foreach (var parameter in errorCase.Parameters)
			{
				sb.Append("\t\tpublic ").Append(parameter.TypeName).Append(' ').Append(parameter.PropertyName).AppendLine(" { get; init; }");
			}
		}
		else
		{
			sb.Append("\t\tpublic static readonly ").Append(errorCase.TypeName).AppendLine(" Instance = new();");
		}

		sb.AppendLine("\t}");
	}
}
