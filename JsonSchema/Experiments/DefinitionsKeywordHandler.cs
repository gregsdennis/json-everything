using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Json.Schema.Experiments;

public class DefinitionsKeywordHandler : IKeywordHandler
{
	public static DefinitionsKeywordHandler Instance { get; } = new();

	public string Name => "definitions";
	public string[]? Dependencies { get; }

	private DefinitionsKeywordHandler() { }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> siblingEvaluations)
	{
		return KeywordEvaluation.Skip;
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => keywordValue is JsonObject a ? [.. a.Select(x => x.Value)] : [];
}