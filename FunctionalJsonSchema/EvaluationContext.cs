using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;
using Json.Pointer;

namespace FunctionalJsonSchema;

public struct EvaluationContext
{
	public Uri BaseUri { get; set; }
	public JsonPointer SchemaLocation { get; set; }
	public JsonPointer InstanceLocation { get; set; }
	public JsonPointer EvaluationPath { get; set; }
	public JsonNode? LocalInstance { get; set; }
	public EvaluationOptions Options { get; set; }

	internal Uri? RefUri { get; set; }

	public EvaluationResults Evaluate(JsonNode? localSchema)
	{
		if (localSchema is JsonValue value)
		{
			var boolSchema = value.GetBool();
			if (!boolSchema.HasValue)
				throw new ArgumentException("Schema must be an object or a boolean");

			return new EvaluationResults
			{
				Valid = boolSchema.Value,
				SchemaLocation = SchemaLocation,
				InstanceLocation = InstanceLocation,
				EvaluationPath = EvaluationPath
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
			if (baseUri.IsAbsoluteUri && !string.IsNullOrEmpty(baseUri.Fragment))
				throw new ArgumentException("$id must not contain a fragment");
			if (!baseUri.IsAbsoluteUri && BaseUri is null)
				baseUri = new Uri(JsonSchema.DefaultBaseUri, baseUri);
		}

		if (baseUri is not null)
		{
			BaseUri = BaseUri is null
				? baseUri
				: new Uri(BaseUri, baseUri);
			SchemaLocation = JsonPointer.Empty;
		}
		else if (RefUri is not null)
			BaseUri = RefUri;

		var valid = true;
		var evaluations = new List<KeywordEvaluation>();
		var annotations = new Dictionary<string, JsonNode?>();
		foreach (var entry in withHandlers)
		{
			var keywordContext = this;
			var keywordResult = entry.Handler?.Handle(entry.Keyword.Value, keywordContext, evaluations) ??
			                    new KeywordEvaluation
			                    {
									Valid = true,
									Annotation = entry.Keyword.Value,
									HasAnnotation = true
			                    };
			valid &= keywordResult.Valid;
			keywordResult.Key = entry.Keyword.Key;
			evaluations.Add(keywordResult);
			if (keywordResult.HasAnnotation)
				annotations[entry.Keyword.Key] = keywordResult.Annotation;
		}

		return new EvaluationResults
		{
			Valid = valid,
			SchemaLocation = SchemaLocation,
			InstanceLocation = InstanceLocation,
			EvaluationPath = EvaluationPath,
			Details = evaluations.Any() ? evaluations.SelectMany(x => x.Children).ToArray() : null,
			Annotations = valid && annotations.Any() ? annotations : null
		};
	}
}