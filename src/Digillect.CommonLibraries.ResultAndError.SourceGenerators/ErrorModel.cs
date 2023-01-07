namespace Digillect.CommonLibraries.ResultAndError.SourceGenerators;

internal record ErrorModel(
	string Name,
	string? Namespace,
	IReadOnlyList<ErrorCase> Cases);

internal record ErrorCase(
	string Name,
	string TypeName,
	bool IsPublicFactory,
	string FactoryReturnTypeName,
	string OriginalParametersDeclaration,
	IReadOnlyList<ErrorCaseParameter> Parameters);

internal record ErrorCaseParameter(string Name, string PropertyName, string TypeName);
