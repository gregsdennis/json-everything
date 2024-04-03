using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class MaxLengthKeywordHandler : IKeywordHandler
{
	public string Name => "maxLength";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is JsonValue value)
		{
			var maximum = value.GetNumber();
			if (maximum.HasValue)
			{
				if (context.LocalInstance is not JsonValue instance) return KeywordEvaluation.Skip;
				var str = instance.GetString();
				if (str is null) return KeywordEvaluation.Skip;

				var length = new StringInfo(str).LengthInTextElements;
				return maximum >= length;
			}
		}

		throw new ArgumentException("'maxLength' keyword must contain a number");
	}
}