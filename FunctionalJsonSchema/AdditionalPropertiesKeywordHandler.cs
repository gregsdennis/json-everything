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

		var properties = instance.Where(x => !propertiesAnnotation!.Contains(x.Key, JsonNodeEqualityComparer.Instance) &&
		                                     !patternPropertiesAnnotation!.Contains(x.Key, JsonNodeEqualityComparer.Instance));

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