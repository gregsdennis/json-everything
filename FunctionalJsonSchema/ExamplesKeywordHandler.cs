using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class ExamplesKeywordHandler : IKeywordHandler
{
	public static ExamplesKeywordHandler Instance { get; } = new();

	public string Name => "examples";
	public string[]? Dependencies { get; }

	private ExamplesKeywordHandler() { }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> siblingEvaluations)
	{
		return KeywordEvaluation.Annotate;
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [];
}