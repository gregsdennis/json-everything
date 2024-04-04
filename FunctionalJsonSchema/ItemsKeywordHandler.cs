using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class ItemsKeywordHandler : IKeywordHandler
{
	public string Name => "items";
	public string[]? Dependencies { get; } = ["prefixItems"];

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> evaluations)
	{
		if (context.LocalInstance is not JsonArray instance) return KeywordEvaluation.Skip;

		var skip = 0;
		if (evaluations.TryGetAnnotation("prefixItems", out JsonValue? prefixItemsAnnotation)) 
			skip = (int?)prefixItemsAnnotation.GetInteger() + 1 ?? 0;

		var contextTemplate = context;
		contextTemplate.EvaluationPath = context.EvaluationPath.Combine(Name);
		contextTemplate.SchemaLocation = context.SchemaLocation.Combine(Name);

		var results = instance.Skip(skip).Select((x, i) =>
		{
			var localContext = contextTemplate;
			localContext.InstanceLocation = localContext.InstanceLocation.Combine(skip + i);
			localContext.LocalInstance = x;

			return (Index: skip + i, Evaluation: localContext.Evaluate(keywordValue));
		}).ToArray();

		return new KeywordEvaluation
		{
			Valid = results.All(x => x.Evaluation.Valid),
			Annotation = results.Any() ? results.Max(x => x.Index) : -1,
			HasAnnotation = results.Any(),
			Children = results.Select(x => x.Evaluation).ToArray()
		};
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [keywordValue];
}