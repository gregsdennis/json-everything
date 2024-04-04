using System.Collections.Generic;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class MultipleOfKeywordHandler : IKeywordHandler
{
	public string Name => "multipleOf";
	public string[]? Dependencies { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is not JsonValue value)
			throw new SchemaValidationException("'multipleOf' keyword must contain a number", context);

		var divisor = value.GetNumber();
		if (!divisor.HasValue)
			throw new SchemaValidationException("'multipleOf' keyword must contain a number", context);

		if (context.LocalInstance is not JsonValue instance) return KeywordEvaluation.Skip;
		var number = instance.GetNumber();
		if (number is null) return KeywordEvaluation.Skip;

		return number % divisor == 0;

	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [];
}