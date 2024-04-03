using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class MaximumKeywordHandler : IKeywordHandler
{
	public string Name => "maximum";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is JsonValue value)
		{
			var maximum = value.GetNumber();
			if (maximum.HasValue)
			{
				if (context.LocalInstance is not JsonValue instance) return KeywordEvaluation.Skip;
				var number = instance.GetNumber();
				if (number is null) return KeywordEvaluation.Skip;

				return maximum >= number;
			}
		}

		throw new ArgumentException("'maximum' keyword must contain a number");
	}
}