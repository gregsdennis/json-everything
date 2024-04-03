using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public interface IKeywordHandler
{
	public string Name { get; }
	public string[]? Dependencies { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations);
}