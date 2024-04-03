using System.Collections.Generic;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class MinimumKeywordHandler : IKeywordHandler
{
	public string Name => "minimum";
	public string[]? Dependencies { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is not JsonValue value)
			throw new SchemaValidationException("'minimum' keyword must contain a number", context);
		
		var minimum = value.GetNumber();
		if (!minimum.HasValue)
			throw new SchemaValidationException("'minimum' keyword must contain a number", context);
		
		if (context.LocalInstance is not JsonValue instance) return KeywordEvaluation.Skip;
		var number = instance.GetNumber();
		if (number is null) return KeywordEvaluation.Skip;

		return minimum <= number;

	}
}