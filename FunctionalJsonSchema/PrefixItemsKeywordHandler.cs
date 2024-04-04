using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class PrefixItemsKeywordHandler : IKeywordHandler
{
	public string Name => "prefixItems";
	public string[]? Dependencies { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> evaluations)
	{
		if (context.LocalInstance is not JsonArray instance) return KeywordEvaluation.Skip;

		if (keywordValue is not JsonArray constraints)
			throw new SchemaValidationException("'prefixItems' keyword must contain an object with schema values", context);

		var items = instance.Zip(constraints, (i, c) => (Item: i, Constraint: c));

		var results = items.Select((x, i) =>
		{
			var localContext = context;
			localContext.InstanceLocation = localContext.InstanceLocation.Combine(i);
			localContext.EvaluationPath = localContext.EvaluationPath.Combine(Name, i);
			localContext.SchemaLocation = localContext.SchemaLocation.Combine(Name, i);
			localContext.LocalInstance = x.Item;

			return (Index: i, Evaluation: localContext.Evaluate(x.Constraint));
		}).ToArray();

		return new KeywordEvaluation
		{
			Valid = results.All(x => x.Evaluation.Valid),
			Annotation = results.Any() ? results.Max(x => x.Index) : -1,
			HasAnnotation = results.Any(),
			Children = results.Select(x => x.Evaluation).ToArray()
		};
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => keywordValue is JsonArray a ? [.. a] : [];
}