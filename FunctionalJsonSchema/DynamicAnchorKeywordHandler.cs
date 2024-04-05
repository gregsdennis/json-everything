using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class DynamicAnchorKeywordHandler : IKeywordHandler
{
	public static DynamicAnchorKeywordHandler Instance { get; } = new();

	public string Name => "$dynamicAnchor";
	public string[]? Dependencies { get; }

	private DynamicAnchorKeywordHandler() { }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> siblingEvaluations)
	{
		return KeywordEvaluation.Skip;
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [];
}