using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class PropertyDependenciesKeywordHandler : IKeywordHandler
{
	public static PropertyDependenciesKeywordHandler Instance { get; } = new();

	public string Name => "propertyDependencies";
	public string[]? Dependencies { get; }

	private PropertyDependenciesKeywordHandler() { }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is not JsonObject constraints)
			throw new SchemaValidationException("'propertyDependencies' keyword must contain an object", context);

		if (context.LocalInstance is not JsonObject instance) return KeywordEvaluation.Skip;

		var properties = instance.Join(constraints,
			i => i.Key,
			c => c.Key,
			(i, c) => (Property: i, Dependencies: c.Value))
			.Select(x =>
			{
				var propertyValue = (x.Property.Value as JsonValue)?.GetString();
				if (propertyValue is null) return (Property: x.Property.Key, Value: null!, Schema: null);

				if (x.Dependencies is not JsonObject dependencies)
					throw new SchemaValidationException("'propertyDependencies' keyword must contain an object of objects", context);

				if (!dependencies.TryGetValue(propertyValue, out var dependency, out _))
					return (Property: x.Property.Key, Value: propertyValue, Schema: null);

				return (Property: x.Property.Key, Value: propertyValue, Schema: dependency);
			})
			.Where(x => x.Schema is not null);

		var results = properties
			.Select(x =>
			{
				var localContext = context;
				localContext.EvaluationPath = context.EvaluationPath.Combine(Name, x.Property, x.Value!);
				localContext.SchemaLocation = context.SchemaLocation.Combine(Name, x.Property, x.Value!);

				return localContext.Evaluate(x.Schema);
			})
			.ToArray();

		return new KeywordEvaluation
		{
			Valid = results.All(x => x.Valid),
			Children = results
		};
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) =>
		keywordValue is JsonObject a
			? [.. a.SelectMany(x => (x.Value as JsonObject)?.Select(y => y.Value) ?? [])]
			: [];
}