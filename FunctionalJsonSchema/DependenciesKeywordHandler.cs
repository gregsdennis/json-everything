using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class DependenciesKeywordHandler : IKeywordHandler
{
	public static DependenciesKeywordHandler Instance { get; } = new();

	public string Name => "dependencies";
	public string[]? Dependencies { get; }

	private DependenciesKeywordHandler() { }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is not JsonObject constraints)
			throw new SchemaValidationException("'dependencies' keyword must contain an object", context);

		if (context.LocalInstance is not JsonObject instance) return KeywordEvaluation.Skip;

		var properties = instance.Join(constraints,
			i => i.Key,
			c => c.Key,
			(i, c) => (Property: i.Key, Constraint: c.Value));

		var results = properties.Select(x =>
		{
			if (x.Constraint is JsonArray requiredArray)
			{
				var required = requiredArray.Select(x => (x as JsonValue)?.GetString()).ToArray();
				if (required.Any(y => y is null))
					throw new SchemaValidationException("'dependencies' keyword must contain an object with string array values", context);

				return (Property: x.Property, Valid: required.All(y => instance.ContainsKey(y!)), Child: null);
			}

			var localContext = context;
			localContext.EvaluationPath = context.EvaluationPath.Combine(Name, x.Property);
			localContext.SchemaLocation = context.SchemaLocation.Combine(Name, x.Property);

			var result = localContext.Evaluate(x.Constraint);

			return (Property: x.Property, Valid: result.Valid, Child: result);
		}).ToArray();

		return new KeywordEvaluation
		{
			Valid = results.All(x => x.Valid),
			Children = results.Where(x => x.Child is not null).Select(x => x.Child!).ToArray()
		};
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [];
}