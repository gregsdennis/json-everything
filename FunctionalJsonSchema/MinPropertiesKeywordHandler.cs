using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class MinPropertiesKeywordHandler : IKeywordHandler
{
	public string Name => "minProperties";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is JsonValue value)
		{
			var minimum = value.GetNumber();
			if (minimum.HasValue)
			{
				if (context.LocalInstance is not JsonObject instance) return KeywordEvaluation.Skip;

				return minimum <= instance.Count;
			}
		}

		throw new ArgumentException("'minProperties' keyword must contain a number");
	}
}