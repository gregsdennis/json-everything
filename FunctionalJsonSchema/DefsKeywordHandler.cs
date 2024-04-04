using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class DefsKeywordHandler : IKeywordHandler
{
	public string Name => "$defs";
	public string[]? Dependencies { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> siblingEvaluations)
	{
		return KeywordEvaluation.Skip;
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => keywordValue is JsonObject a ? [.. a.Select(x => x.Value)] : [];
}