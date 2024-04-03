using System.Collections.Generic;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class ConstKeywordHandler : IKeywordHandler
{
	public string Name => "const";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		return context.LocalInstance.IsEquivalentTo(keywordValue);
	}
}