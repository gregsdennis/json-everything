﻿using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class MaxPropertiesKeywordHandler : IKeywordHandler
{
	public string Name => "maxProperties";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is JsonValue value)
		{
			var maximum = value.GetNumber();
			if (maximum.HasValue)
			{
				if (context.LocalInstance is not JsonObject instance) return KeywordEvaluation.Skip;

				return maximum >= instance.Count;
			}
		}

		throw new ArgumentException("'maxProperties' keyword must contain a number");
	}
}