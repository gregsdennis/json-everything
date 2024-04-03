using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class MinimumKeywordHandler : IKeywordHandler
{
	public string Name => "minimum";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is JsonValue value)
		{
			var minimum = value.GetNumber();
			if (minimum.HasValue)
			{
				if (context.LocalInstance is not JsonValue instance) return KeywordEvaluation.Skip;
				var number = instance.GetNumber();
				if (number is null) return KeywordEvaluation.Skip;

				return minimum <= number;
			}
		}

		throw new ArgumentException("'minimum' keyword must contain a number");
	}
}