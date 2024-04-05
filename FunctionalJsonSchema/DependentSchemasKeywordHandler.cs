using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class DependentSchemasKeywordHandler : IKeywordHandler
{
	public static DependentSchemasKeywordHandler Instance { get; } = new();

	public string Name => "dependentSchemas";
	public string[]? Dependencies { get; }

	private DependentSchemasKeywordHandler() { }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is not JsonObject constraints)
			throw new SchemaValidationException("'dependentSchemas' keyword must contain an object with string array values", context);

		if (context.LocalInstance is not JsonObject instance) return KeywordEvaluation.Skip;

		var properties = instance.Join(constraints,
			i => i.Key,
			c => c.Key,
			(i, c) => (Property: i.Key, Schema: c.Value));

		var results = properties
			.Select(x =>
			{
				var localContext = context;
				localContext.EvaluationPath = context.EvaluationPath.Combine(Name, x.Property);
				localContext.SchemaLocation = context.SchemaLocation.Combine(Name, x.Property);

				return localContext.Evaluate(x.Schema);
			})
			.ToArray();

		return new KeywordEvaluation
		{
			Valid = results.All(x => x.Valid),
			Children = results
		};

	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => keywordValue is JsonObject a ? [.. a.Select(x => x.Value)] : [];
}