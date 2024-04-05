using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class WriteOnlyKeywordHandler : IKeywordHandler
{
	public static WriteOnlyKeywordHandler Instance { get; } = new();

	public string Name => "writeOnly";
	public string[]? Dependencies { get; }

	private WriteOnlyKeywordHandler() { }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> siblingEvaluations)
	{
		return KeywordEvaluation.Annotate;
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [];
}