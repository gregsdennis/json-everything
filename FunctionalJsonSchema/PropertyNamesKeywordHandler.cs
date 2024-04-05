using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class PropertyNamesKeywordHandler : IKeywordHandler
{
	public static PropertyNamesKeywordHandler Instance { get; } = new();

	public string Name => "propertyNames";
	public string[]? Dependencies { get; }

	private PropertyNamesKeywordHandler() { }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> evaluations)
	{
		if (context.LocalInstance is not JsonObject instance) return KeywordEvaluation.Skip;

		var contextTemplate = context;
		contextTemplate.EvaluationPath = context.EvaluationPath.Combine(Name);
		contextTemplate.SchemaLocation = context.SchemaLocation.Combine(Name);

		var results = instance.Select(x =>
		{
			var localContext = contextTemplate;
			localContext.InstanceLocation = localContext.InstanceLocation.Combine(x.Key);
			localContext.LocalInstance = x.Key;

			return localContext.Evaluate(keywordValue);
		}).ToArray();

		return new KeywordEvaluation
		{
			Valid = results.All(x => x.Valid),
			Children = [.. results]
		};
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [keywordValue];
}