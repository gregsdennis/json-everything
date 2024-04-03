using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class ContainsKeywordHandler : IKeywordHandler
{
	public string Name => "contains";
	public string[]? Dependencies { get; } = ["minContains", "maxContains"];

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> evaluations)
	{
		if (context.LocalInstance is not JsonArray instance) return KeywordEvaluation.Skip;

		var minContains = 1;
		if (evaluations.TryGetAnnotation("minContains", out JsonValue? minContainsAnnotation)) 
			minContains = (int?)minContainsAnnotation.GetInteger() ?? 1;
		var maxContains = int.MaxValue;
		if (evaluations.TryGetAnnotation("maxContains", out JsonValue? maxContainsAnnotation))
			maxContains = (int?)maxContainsAnnotation.GetInteger() ?? int.MaxValue;

		var contextTemplate = context;
		contextTemplate.EvaluationPath = context.EvaluationPath.Combine(Name);
		contextTemplate.SchemaLocation = context.SchemaLocation.Combine(Name);

		var results = instance.Select((x, i) =>
		{
			var localContext = contextTemplate;
			localContext.InstanceLocation = localContext.InstanceLocation.Combine(i);
			localContext.LocalInstance = x;

			return (Index: i, Evaluation: localContext.Evaluate(keywordValue));
		}).ToArray();

		var validIndices = results
			.Where(x => x.Evaluation.Valid)
			.Select(x => (JsonNode) x.Index)
			.ToJsonArray();

		return new KeywordEvaluation
		{
			Valid = minContains <= validIndices.Count && validIndices.Count <= maxContains,
			Annotation = validIndices.Count == instance.Count ? true : validIndices,
			HasAnnotation = validIndices.Any(),
			Children = results.Select(x => x.Evaluation).ToArray()
		};
	}
}