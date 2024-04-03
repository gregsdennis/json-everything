using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class EnumKeywordHandler : IKeywordHandler
{
	public string Name => "enum";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is not JsonArray { Count: > 0 } value)
			throw new SchemaValidationException("'enum' keyword must contain an array", context);

		return value.Contains(context.LocalInstance, JsonNodeEqualityComparer.Instance);

	}
}