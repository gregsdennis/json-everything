using System;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class PropertiesKeywordHandler : IKeywordHandler
{
	public string Name => "properties";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context)
	{
		if (context.LocalInstance is not JsonObject instance) return KeywordEvaluation.Skip;

		if (keywordValue is not JsonObject constraints)
			throw new ArgumentException("'properties' keyword must contain an object with schema values");

		var properties = instance.Join(constraints,
			p => p.Key,
			c => c.Key,
			(p, c) => (Property: p, Constraint: c));

		var results = properties.Select(x =>
		{
			var localContext = context;
			localContext.InstanceLocation = localContext.InstanceLocation.Combine(x.Property.Key);
			localContext.EvaluationPath = localContext.InstanceLocation.Combine("properties", x.Property.Key);
			localContext.SchemaLocation = localContext.SchemaLocation.Combine("properties", x.Property.Key);
			localContext.LocalInstance = x.Property.Value;

			return (Key: (JsonNode)x.Property.Key, Evaluation: JsonSchema.Evaluate(x.Constraint.Value, localContext));
		}).ToArray();

		return new KeywordEvaluation
		{
			Valid = results.All(x => x.Evaluation.Valid),
			Annotation = results.Select(x => x.Key).ToJsonArray(),
			HasAnnotation = true,
			Children = results.Select(x => x.Evaluation).ToArray()
		};
	}
}