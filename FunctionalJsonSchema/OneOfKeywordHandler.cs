﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class OneOfKeywordHandler : IKeywordHandler
{
	public string Name => "oneOf";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> evaluations)
	{
		if (keywordValue is not JsonArray constraints)
			throw new SchemaValidationException("'oneOf' keyword must contain an array of schemas", context);

		var results = constraints.Select((x, i) =>
		{
			var localContext = context;
			localContext.EvaluationPath = localContext.EvaluationPath.Combine(Name, i);
			localContext.SchemaLocation = localContext.SchemaLocation.Combine(Name, i);

			return localContext.Evaluate(x);
		}).ToArray();

		return new KeywordEvaluation
		{
			Valid = results.Count(x => x.Valid) == 1,
			Children = [.. results]
		};
	}
}