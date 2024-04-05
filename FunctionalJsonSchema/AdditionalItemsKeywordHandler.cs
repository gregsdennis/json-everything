using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class AdditionalItemsKeywordHandler : IKeywordHandler
{
	public static AdditionalItemsKeywordHandler Instance { get; } = new();

	public string Name => "additionalItems";
	public string[]? Dependencies { get; } = ["items"];

	private AdditionalItemsKeywordHandler(){}

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> evaluations)
	{
		if (context.LocalInstance is not JsonArray instance) return KeywordEvaluation.Skip;

		int skip;
		if (evaluations.TryGetAnnotation("items", out JsonValue? itemsAnnotation))
			skip = (int?)itemsAnnotation.GetInteger() + 1 ?? 0;
		else
			return KeywordEvaluation.Skip;

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