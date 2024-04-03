using System.Collections.Generic;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class MinPropertiesKeywordHandler : IKeywordHandler
{
	public string Name => "minProperties";
	public string[]? Dependencies { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is not JsonValue value)
			throw new SchemaValidationException("'minProperties' keyword must contain a number", context);
		
		var minimum = value.GetNumber();
		if (!minimum.HasValue)
			throw new SchemaValidationException("'minProperties' keyword must contain a number", context);
		
		if (context.LocalInstance is not JsonObject instance) return KeywordEvaluation.Skip;

		return minimum <= instance.Count;

	}
}