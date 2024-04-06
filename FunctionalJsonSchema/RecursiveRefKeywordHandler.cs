using System.Collections.Generic;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class RecursiveRefKeywordHandler : IKeywordHandler
{
	public static RecursiveRefKeywordHandler Instance { get; } = new();

	public string Name => "$recursiveRef";
	public string[]? Dependencies { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> evaluations)
	{
		var reference = (keywordValue as JsonValue)?.GetString();
		if (reference is not "#")
			throw new SchemaValidationException("$recursiveRef may only be '#'", context);

		// TODO handle navigation loops
		var search = context.Options.SchemaRegistry.GetRecursive(context.DynamicScope);
		var (target, newBaseUri) = search;

		var localContext = context;
		localContext.RefUri = newBaseUri;
		localContext.EvaluationPath = localContext.EvaluationPath.Combine(Name);
		localContext.SchemaLocation = localContext.SchemaLocation.Combine(Name);

		var result = localContext.Evaluate(target);

		return new KeywordEvaluation
		{
			Valid = result.Valid,
			Children = [result]
		};
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [];
}