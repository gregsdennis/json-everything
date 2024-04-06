using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Json.More;

namespace Json.Schema.Experiments;

public class PatternPropertiesKeywordHandler : IKeywordHandler
{
	public static PatternPropertiesKeywordHandler Instance { get; } = new();

	public string Name => "patternProperties";
	public string[]? Dependencies { get; }

	private PatternPropertiesKeywordHandler() { }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> evaluations)
	{
		if (context.LocalInstance is not JsonObject instance) return KeywordEvaluation.Skip;

		if (keywordValue is not JsonObject constraints)
			throw new SchemaValidationException("'patternProperties' keyword must contain an object with schema values", context);

		var properties = instance.SelectMany(_ => constraints, (i, c) => (i, c))
			.Where(x => Regex.IsMatch(x.i.Key, x.c.Key, RegexOptions.ECMAScript))
			.Select(x => (Property: x.i, Constraint: x.c));

		var results = properties.Select(x =>
		{
			var localContext = context;
			localContext.InstanceLocation = localContext.InstanceLocation.Combine(x.Property.Key);
			localContext.EvaluationPath = localContext.EvaluationPath.Combine(Name, x.Property.Key);
			localContext.SchemaLocation = localContext.SchemaLocation.Combine(Name, x.Property.Key);
			localContext.LocalInstance = x.Property.Value;

			return (Key: (JsonNode)x.Property.Key, Evaluation: localContext.Evaluate(x.Constraint.Value));
		}).ToArray();

		return new KeywordEvaluation
		{
			Valid = results.All(x => x.Evaluation.Valid),
			Annotation = results.Select(x => x.Key).ToJsonArray(),
			HasAnnotation = results.Any(),
			Children = results.Select(x => x.Evaluation).ToArray()
		};
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => keywordValue is JsonObject a ? [.. a.Select(x => x.Value)] : [];
}