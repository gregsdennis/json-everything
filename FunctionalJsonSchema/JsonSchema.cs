using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;
using Json.Pointer;

namespace FunctionalJsonSchema;

public class JsonSchema
{
	public static Uri DefaultBaseUri { get; set; } = new("https://json-everything.net/");

	public static EvaluationResults Evaluate(JsonNode schema, JsonNode? instance)
	{
		if (schema is JsonObject objSchema && !objSchema.ContainsKey("$id"))
			objSchema["$id"] = GenerateId().OriginalString;

		var context = new EvaluationContext
		{
			SchemaLocation = JsonPointer.Empty,
			InstanceLocation = JsonPointer.Empty,
			EvaluationPath = JsonPointer.Empty,
			LocalInstance = instance
		};

		return Evaluate(schema, context);
	}

	public static EvaluationResults Evaluate(JsonNode? localSchema, EvaluationContext context)
	{
		if (localSchema is JsonValue value)
		{
			var boolSchema = value.GetBool();
			if (!boolSchema.HasValue)
				throw new ArgumentException("Schema must be an object or a boolean");

			return new EvaluationResults
			{
				Valid = boolSchema.Value
			};
		}

		if (localSchema is not JsonObject objSchema)
			throw new ArgumentException("Schema must be an object or a boolean");

		var withHandlers = KeywordRegistry.GetHandlers(objSchema);

		Uri? baseUri = null;
		if (objSchema.TryGetValue("$id", out var idNode, out _))
		{
			var id = (idNode as JsonValue)?.GetString();
			if (!Uri.TryCreate(id, UriKind.RelativeOrAbsolute, out baseUri))
				throw new ArgumentException("$id must be a valid URI");
			if (!string.IsNullOrEmpty(baseUri.Fragment))
				throw new ArgumentException("$id must not contain a fragment");
			if (!baseUri.IsAbsoluteUri)
				baseUri = new Uri(DefaultBaseUri, baseUri);
		}

		if (baseUri is not null)
		{
			context.BaseUri = baseUri;
			context.SchemaLocation = JsonPointer.Empty;
		}

		var valid = true;
		var details = new List<EvaluationResults>();
		var annotations = new JsonObject();
		foreach (var entry in withHandlers)
		{
			if (entry.Handler is null) continue;

			var keywordContext = context;
			var keywordResult = entry.Handler.Handle(entry.Keyword.Value, keywordContext);
			valid &= keywordResult.Valid;
			details.AddRange(keywordResult.Children);
			if (keywordResult.HasAnnotation)
				annotations[entry.Keyword.Key] = keywordResult.Annotation;
		}

		return new EvaluationResults
		{
			Valid = valid,
			SchemaLocation = context.SchemaLocation,
			InstanceLocation = context.InstanceLocation,
			EvaluationPath = context.EvaluationPath,
			Details = details.Any() ? [.. details] : null,
			Annotations = valid && annotations.Any() ? annotations : null
		};
	}

	private static Uri GenerateId() => new(DefaultBaseUri, Guid.NewGuid().ToString("N")[..10]);
}

#if NETSTANDARD2_0
#endif