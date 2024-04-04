using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class SchemaKeywordHandler : IKeywordHandler
{
	public string Name => "$schema";
	public string[]? Dependencies { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> siblingEvaluations)
	{
		return KeywordEvaluation.Skip;
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [];
}