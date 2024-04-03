using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class UniqueItemsKeywordHandler : IKeywordHandler
{
	public string Name => "uniqueItems";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is not JsonValue value)
			throw new SchemaValidationException("'uniqueItems' keyword must contain a number", context);
		
		var unique = value.GetBool();
		if (!unique.HasValue)
			throw new SchemaValidationException("'uniqueItems' keyword must contain a number", context);
		
		if (context.LocalInstance is not JsonArray instance) return KeywordEvaluation.Skip;

		return unique == false || instance.Count == instance.Distinct(JsonNodeEqualityComparer.Instance).Count();
	}
}