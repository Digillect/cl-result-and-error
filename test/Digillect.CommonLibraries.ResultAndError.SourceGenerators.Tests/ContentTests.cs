namespace Digillect.CommonLibraries.ResultAndError.SourceGenerators.Tests;

public sealed class ContentTests
{
	[Theory]
	[InlineData("AccessibilityTestsErrors.cs")]
	public void It_finds_embedded_resource_with_the_source_from(string sourceName)
	{
		var assembly = typeof(ContentTests).Assembly;

		using var stream = assembly.GetManifestResourceStream($"Digillect.CommonLibraries.ResultAndError.SourceGenerators.Tests.Sources.{sourceName}");

		Assert.NotNull(stream);
	}
}
