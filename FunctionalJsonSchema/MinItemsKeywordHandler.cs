using System.Collections.Generic;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class MinItemsKeywordHandler : IKeywordHandler
{
	public string Name => "minItems";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is not JsonValue value)
			throw new SchemaValidationException("'minItems' keyword must contain a number", context);
		
		var minimum = value.GetNumber();
		if (!minimum.HasValue) 
			throw new SchemaValidationException("'minItems' keyword must contain a number", context);
		
		if (context.LocalInstance is not JsonArray instance) return KeywordEvaluation.Skip;

		return minimum <= instance.Count;

	}
}