using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class VocabularyKeywordHandler : IKeywordHandler
{
	public static VocabularyKeywordHandler Instance { get; } = new();

	public string Name => "$vocabulary";
	public string[]? Dependencies { get; }

	private VocabularyKeywordHandler() { }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> siblingEvaluations)
	{
		return KeywordEvaluation.Skip;
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [];
}