using System.Collections.Generic;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class MaxItemsKeywordHandler : IKeywordHandler
{
	public string Name => "maxItems";
	public string[]? Dependencies { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is not JsonValue value)
			throw new SchemaValidationException("'maxItems' keyword must contain a number", context);
		
		var maximum = value.GetNumber();
		if (!maximum.HasValue)
			throw new SchemaValidationException("'maxItems' keyword must contain a number", context);
		
		if (context.LocalInstance is not JsonArray instance) return KeywordEvaluation.Skip;

		return maximum >= instance.Count;

	}
}