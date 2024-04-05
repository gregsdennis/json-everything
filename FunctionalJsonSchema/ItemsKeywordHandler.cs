using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class ItemsKeywordHandler : IKeywordHandler
{
	private readonly bool _allowArrays;

	public static ItemsKeywordHandler OnlySingle { get; } = new(false);
	public static ItemsKeywordHandler AllowArrays { get; } = new(true);

	public string Name => "items";
	public string[]? Dependencies { get; } = ["prefixItems"];

	private ItemsKeywordHandler(bool allowArrays)
	{
		_allowArrays = allowArrays;
	}

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> evaluations)
	{
		if (context.LocalInstance is not JsonArray instance) return KeywordEvaluation.Skip;

		if (keywordValue is JsonObject || keywordValue is JsonValue v && v.GetBool() is not null)
			return HandleSingle(keywordValue, context, evaluations, instance);

		if (!_allowArrays)
			throw new SchemaValidationException("items must be a schema", context);

		if (keywordValue is JsonArray arr)
			return HandleArray(arr, context, instance);

		throw new SchemaValidationException("items must be either a schema or an array of schemas", context);
	}

	private KeywordEvaluation HandleSingle(JsonNode keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> evaluations, JsonArray instance)
	{
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

	private KeywordEvaluation HandleArray(JsonArray constraints, EvaluationContext context, JsonArray instance)
	{
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

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [keywordValue];
}