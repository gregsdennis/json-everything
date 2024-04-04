﻿using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class MaxContainsKeywordHandler : IKeywordHandler
{
	public string Name => "maxContains";
	public string[]? Dependencies { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> evaluations)
	{
		return new KeywordEvaluation
		{
			Valid = true,
			Annotation = keywordValue,
			HasAnnotation = true
		};
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [];
}