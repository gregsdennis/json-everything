using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class AdditionalPropertiesKeywordHandler : IKeywordHandler
{
	public string Name => "additionalProperties";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> evaluations)
	{
		if (context.LocalInstance is not JsonObject instance) return KeywordEvaluation.Skip;

		if (!evaluations.TryGetAnnotation("properties", out JsonArray? propertiesAnnotation))
			propertiesAnnotation = [];
		if (!evaluations.TryGetAnnotation("patternProperties", out JsonArray? patternPropertiesAnnotation))
			patternPropertiesAnnotation = [];

		var evaluatedProperties = propertiesAnnotation
			.Concat(patternPropertiesAnnotation)
			.OfType<JsonValue>()
			.Select(x => x.GetString())
			.ToArray();

		var properties = instance.Where(x => !evaluatedProperties!.Contains(x.Key));

		var contextTemplate = context;
		contextTemplate.EvaluationPath = context.EvaluationPath.Combine(Name);
		contextTemplate.SchemaLocation = context.SchemaLocation.Combine(Name);

		var results = properties.Select(x =>
		{
			var localContext = contextTemplate;
			localContext.InstanceLocation = localContext.InstanceLocation.Combine(x.Key);
			localContext.LocalInstance = x.Value;

			return (Key: (JsonNode)x.Key, Evaluation: localContext.Evaluate(keywordValue));
		}).ToArray();

		return new KeywordEvaluation
		{
			Valid = results.All(x => x.Evaluation.Valid),
			Annotation = results.Select(x => x.Key).ToJsonArray(),
			HasAnnotation = results.Any(),
			Children = results.Select(x => x.Evaluation).ToArray()
		};
	}
}

public class ItemsKeywordHandler : IKeywordHandler
{
	public string Name => "items";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> evaluations)
	{
		if (context.LocalInstance is not JsonArray instance) return KeywordEvaluation.Skip;

		var skip = 0;
		if (evaluations.TryGetAnnotation("prefixItems", out JsonValue? prefixItemsAnnotation))
		{
			if (prefixItemsAnnotation.GetBool() == true) return KeywordEvaluation.Skip;

			skip = (int?)prefixItemsAnnotation.GetInteger() + 1 ?? 0;
		}

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
			Annotation = instance.Count == results.Length + skip ? true : results.Max(x => x.Index),
			HasAnnotation = results.Any(),
			Children = results.Select(x => x.Evaluation).ToArray()
		};
	}
}


public class UnevaluatedItemsKeywordHandler : IKeywordHandler
{
	public string Name => "unevaluatedItems";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> evaluations)
	{
		if (context.LocalInstance is not JsonArray instance) return KeywordEvaluation.Skip;

		var indexAnnotations = evaluations.GetAllAnnotations<JsonValue>("prefixItems")
			.Concat(evaluations.GetAllAnnotations<JsonValue>("items"))
			.Concat(evaluations.GetAllAnnotations<JsonValue>("unevaluatedItems"))
			.ToArray();

		if (indexAnnotations.Any(x => x.GetBool() == true)) return KeywordEvaluation.Skip;

		var skip = indexAnnotations.Any() ? indexAnnotations.Max(x => (int?)x.GetInteger() ?? 0) : 0;

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
			Annotation = instance.Count == results.Length + skip ? true : results.Max(x => x.Index),
			HasAnnotation = results.Any(),
			Children = results.Select(x => x.Evaluation).ToArray()
		};
	}
}

public class PrefixItemsKeywordHandler : IKeywordHandler
{
	public string Name => "prefixItems";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> evaluations)
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
			Annotation = instance.Count == results.Length ? true : results.Max(x => x.Index),
			HasAnnotation = results.Any(),
			Children = results.Select(x => x.Evaluation).ToArray()
		};
	}
}