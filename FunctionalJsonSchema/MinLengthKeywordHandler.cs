using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class MinLengthKeywordHandler : IKeywordHandler
{
	public string Name => "minLength";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is JsonValue value)
		{
			var minimum = value.GetNumber();
			if (minimum.HasValue)
			{
				if (context.LocalInstance is not JsonValue instance) return KeywordEvaluation.Skip;
				var str = instance.GetString();
				if (str is null) return KeywordEvaluation.Skip;

				var length = new StringInfo(str).LengthInTextElements;
				return minimum <= length;
			}
		}

		throw new ArgumentException("'minLength' keyword must contain a number");
	}
}