using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class MaxItemsKeywordHandler : IKeywordHandler
{
	public string Name => "maxItems";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is JsonValue value)
		{
			var maximum = value.GetNumber();
			if (maximum.HasValue)
			{
				if (context.LocalInstance is not JsonArray instance) return KeywordEvaluation.Skip;

				return maximum >= instance.Count;
			}
		}

		throw new ArgumentException("'maxItems' keyword must contain a number");
	}
}