using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class AnchorKeywordHandler : IKeywordHandler
{
	public static AnchorKeywordHandler Instance { get; } = new();

	public string Name => "$anchor";
	public string[]? Dependencies { get; }

	private AnchorKeywordHandler() { }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> siblingEvaluations)
	{
		return KeywordEvaluation.Skip;
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [];
}