using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Digillect.CommonLibraries.ResultAndError.SourceGenerators.Tests;

[UsesVerify]
public sealed class GeneratorTests
{
	[Fact]
	public Task It_generates_code_for_public_abstract_partial_classes()
	{
		string? result = GetGeneratedOutput("PublicAbstractPartialError.cs");

		Assert.NotNull(result);

		return Verify(result);
	}

	[Fact]
	public Task It_generates_code_for_public_partial_classes()
	{
		string? result = GetGeneratedOutput("PublicPartialError.cs");

		Assert.NotNull(result);

		return Verify(result);
	}

	[Fact]
	public Task It_generates_code_for_internal_abstract_partial_classes()
	{
		string? result = GetGeneratedOutput("InternalAbstractPartialError.cs");

		Assert.NotNull(result);

		return Verify(result);
	}

	[Fact]
	public Task It_generates_code_for_internal_partial_classes()
	{
		string? result = GetGeneratedOutput("InternalPartialError.cs");

		Assert.NotNull(result);

		return Verify(result);
	}

	[Fact]
	public void It_does_not_generate_code_for_public_abstract_classes_that_are_not_partial()
	{
		var result = GetGeneratedSyntaxTree("PublicAbstractError.cs");

		Assert.Null(result);
	}

	private static string? GetGeneratedOutput(string sourceName)
	{
		var syntaxTree = GetGeneratedSyntaxTree(sourceName);

		return syntaxTree?.ToString();
	}

	private static SyntaxTree? GetGeneratedSyntaxTree(string sourceName)
	{
		var currentAssembly = typeof(GeneratorTests).Assembly;

		using var stream = currentAssembly.GetManifestResourceStream($"Digillect.CommonLibraries.ResultAndError.SourceGenerators.Tests.Sources.{sourceName}");

		Assert.NotNull(stream);

		using var reader = new StreamReader(stream!);

		var syntaxTree = CSharpSyntaxTree.ParseText(reader.ReadToEnd());
		var unused = typeof(Error); // This is required to have a reference to the assembly

		var references = new List<MetadataReference>();
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

		foreach (var assembly in assemblies)
		{
			if (!assembly.IsDynamic)
			{
				references.Add(MetadataReference.CreateFromFile(assembly.Location));
			}
		}

		var compilation = CSharpCompilation.Create(
			"foo",
			new [] { syntaxTree },
			references,
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

		// TODO: Uncomment these lines if you want to return immediately if the injected program isn't valid _before_ running generators
		//
		// ImmutableArray<Diagnostic> compilationDiagnostics = compilation.GetDiagnostics();
		//
		// if (diagnostics.Any())
		// {
		//     return (diagnostics, "");
		// }

		var generator = new ErrorImplementationGenerator();

		var driver = CSharpGeneratorDriver.Create(generator);
		driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generateDiagnostics);

		Assert.Empty(generateDiagnostics);

		if (outputCompilation.SyntaxTrees.TryGetNonEnumeratedCount(out int count))
		{
			return count < 2 ? null : outputCompilation.SyntaxTrees.Last();
		}

		var trees = outputCompilation.SyntaxTrees.ToArray();

		return trees.Length < 2 ? null : trees[^1];
	}
}
