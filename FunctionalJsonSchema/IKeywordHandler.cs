using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public interface IKeywordHandler
{
	public string Name { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context);
}