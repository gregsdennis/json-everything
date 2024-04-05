using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class DependentRequiredKeywordHandler : IKeywordHandler
{
	public static DependentRequiredKeywordHandler Instance { get; } = new();

	public string Name => "dependentRequired";
	public string[]? Dependencies { get; }

	private DependentRequiredKeywordHandler() { }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is not JsonObject constraints)
			throw new SchemaValidationException("'dependentRequired' keyword must contain an object with string array values", context);

		if (context.LocalInstance is not JsonObject instance) return KeywordEvaluation.Skip;

		var properties = instance.Join(constraints,
			i => i.Key,
			c => c.Key,
			(i, c) => (Property: i.Key, Required: c.Value));

		var results = properties.Select(x =>
		{
			if (x.Required is not JsonArray requiredArray)
				throw new SchemaValidationException("'dependentRequired' keyword must contain an object with string array values", context);

			var required = requiredArray.Select(x => (x as JsonValue)?.GetString()).ToArray();
			if (required.Any(y => y is null))
				throw new SchemaValidationException("'dependentRequired' keyword must contain an object with string array values", context);

			return (Property: x.Property, Valid: required.All(y => instance.ContainsKey(y!)));
		});

		return results.All(x => x.Valid);

	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [];
}