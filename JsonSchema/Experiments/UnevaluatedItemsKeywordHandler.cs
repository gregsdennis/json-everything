using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace Json.Schema.Experiments;

public class UnevaluatedItemsKeywordHandler : IKeywordHandler
{
	public static UnevaluatedItemsKeywordHandler Instance { get; } = new();

	public string Name => "unevaluatedItems";
	public string[]? Dependencies { get; } = ["additionalItems", "contains", "prefixItems", "items", "unevaluatedItems"];

	private UnevaluatedItemsKeywordHandler() { }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> evaluations)
	{
		if (context.LocalInstance is not JsonArray instance) return KeywordEvaluation.Skip;

		var indexAnnotations = evaluations.GetAllAnnotations<JsonValue>("prefixItems")
			.Concat(evaluations.GetAllAnnotations<JsonValue>("items"))
			.Concat(evaluations.GetAllAnnotations<JsonValue>("additionalItems"))
			.Concat(evaluations.GetAllAnnotations<JsonValue>("unevaluatedItems"))
			.ToArray();

		var containsIndices = evaluations.GetAllAnnotations<JsonArray>("contains")
			.SelectMany(x => x.Select(y => (y as JsonValue)?.GetInteger()))
			.Where(x => x is not null)
			.ToArray();

		var skip = indexAnnotations.Any() ? indexAnnotations.Max(x => (int?)x.GetInteger() ?? 0) + 1 : 0;

		var contextTemplate = context;
		contextTemplate.EvaluationPath = context.EvaluationPath.Combine(Name);
		contextTemplate.SchemaLocation = context.SchemaLocation.Combine(Name);

		var results = instance.Skip(skip)
			.Where((_, i) => !containsIndices.Contains(i + skip))
			.Select((x, i) =>
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