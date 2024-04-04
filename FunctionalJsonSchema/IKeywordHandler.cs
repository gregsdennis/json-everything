using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public interface IKeywordHandler
{
	public string Name { get; }
	public string[]? Dependencies { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> siblingEvaluations);
	public JsonNode?[] GetSubschemas(JsonNode? keywordValue);
}