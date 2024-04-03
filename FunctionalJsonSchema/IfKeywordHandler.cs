using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class IfKeywordHandler : IKeywordHandler
{
	public string Name => "if";
	public string[]? Dependencies { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> evaluations)
	{
		var localContext = context;
		localContext.EvaluationPath = localContext.EvaluationPath.Combine(Name);
		localContext.SchemaLocation = localContext.SchemaLocation.Combine(Name);

		var result = localContext.Evaluate(keywordValue);

		return new KeywordEvaluation
		{
			Valid = true,
			Children = [result]
		};
	}
}