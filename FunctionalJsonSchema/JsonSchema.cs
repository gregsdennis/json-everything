using System;
using System.Text.Json.Nodes;
using Json.Pointer;

namespace FunctionalJsonSchema;

public static class JsonSchema
{
	public static Uri DefaultBaseUri { get; set; } = new("https://json-everything.net/");

	public static EvaluationResults Evaluate(JsonNode schema, JsonNode? instance)
	{
		if (schema is JsonObject objSchema && !objSchema.ContainsKey("$id"))
		{
			objSchema = (JsonObject)objSchema.DeepClone();
			objSchema["$id"] = GenerateId().OriginalString;
		}

		var context = new EvaluationContext
		{
			SchemaLocation = JsonPointer.Empty,
			InstanceLocation = JsonPointer.Empty,
			EvaluationPath = JsonPointer.Empty,
			LocalInstance = instance
		};

		return context.Evaluate(schema);
	}

	private static Uri GenerateId() => new(DefaultBaseUri, Guid.NewGuid().ToString("N")[..10]);
}
