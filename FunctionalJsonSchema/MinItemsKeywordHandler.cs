using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class MinItemsKeywordHandler : IKeywordHandler
{
	public string Name => "minItems";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is JsonValue value)
		{
			var minimum = value.GetNumber();
			if (minimum.HasValue)
			{
				if (context.LocalInstance is not JsonArray instance) return KeywordEvaluation.Skip;

				return minimum <= instance.Count;
			}
		}

		throw new ArgumentException("'minItems' keyword must contain a number");
	}
}