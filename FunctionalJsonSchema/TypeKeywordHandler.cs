using System;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class TypeKeywordHandler : IKeywordHandler
{
	public string Name => "type";

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context)
	{
		var instanceType = context.LocalInstance.GetSchemaValueType();
		if (keywordValue is JsonValue value)
		{
			// still need keyword validation
			return new KeywordEvaluation
			{
				Valid = value.IsEquivalentTo(instanceType)
			};
		}
		if (keywordValue is JsonArray multipleTypes)
		{
			return new KeywordEvaluation
			{
				Valid = multipleTypes.Any(x => Json.More.JsonNodeExtensions.IsEquivalentTo(x, instanceType))
			};
		}

		throw new ArgumentException("'type' keyword must contain a valid JSON Schema type or an array of valid JSON Schema types");
	}
}