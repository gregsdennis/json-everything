using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class NotKeywordHandler : IKeywordHandler
{
	public string Name => "not";
	public string[]? Dependencies { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> evaluations)
	{
		var localContext = context;
		localContext.EvaluationPath = localContext.EvaluationPath.Combine(Name);
		localContext.SchemaLocation = localContext.SchemaLocation.Combine(Name);

		var result = localContext.Evaluate(keywordValue);

		return new KeywordEvaluation
		{
			Valid = !result.Valid,
			Children = [result]
		};
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [keywordValue];
}