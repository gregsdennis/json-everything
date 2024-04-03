using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class UnevaluatedPropertiesKeywordHandler : IKeywordHandler
{
	public string Name => "unevaluatedProperties";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> evaluations)
	{
		if (context.LocalInstance is not JsonObject instance) return KeywordEvaluation.Skip;

		var propertiesAnnotations = evaluations.GetAllAnnotations<JsonArray>("properties");
		var patternPropertiesAnnotations = evaluations.GetAllAnnotations<JsonArray>("patternProperties");
		var additionalPropertiesAnnotations = evaluations.GetAllAnnotations<JsonArray>("additionalProperties");
		var unevaluatedPropertiesAnnotations = evaluations.GetAllAnnotations<JsonArray>("unevaluatedProperties");

		var evaluatedProperties = propertiesAnnotations
			.Concat(patternPropertiesAnnotations)
			.Concat(additionalPropertiesAnnotations)
			.Concat(unevaluatedPropertiesAnnotations)
			.SelectMany(x => x)
			.OfType<JsonValue>()
			.Select(x => x.GetString())
			.ToArray();

		var properties = instance.Where(x => !evaluatedProperties.Contains(x.Key));

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