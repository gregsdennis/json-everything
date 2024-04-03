using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class RequiredKeywordHandler : IKeywordHandler
{
	public string Name => "required";
	public string[]? Dependencies { get; }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyList<KeywordEvaluation> siblingEvaluations)
	{
		if (keywordValue is not JsonArray value)
			throw new SchemaValidationException("'required' keyword must contain an array of strings", context);
		
		var required = value.Select(x => (x as JsonValue)?.GetString()).ToArray();
		if (!required.All(x => x is not null))
			throw new SchemaValidationException("'required' keyword must contain an array of strings", context);
		
		if (context.LocalInstance is not JsonObject instance) return KeywordEvaluation.Skip;

		return required.All(x => instance.ContainsKey(x!));

	}
}