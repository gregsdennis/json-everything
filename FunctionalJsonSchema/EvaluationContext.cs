using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;
using Json.Pointer;

namespace FunctionalJsonSchema;

public struct EvaluationContext
{
	public Uri BaseUri { get; private set; }
	public JsonPointer SchemaLocation { get; set; }
	public JsonPointer InstanceLocation { get; set; }
	public JsonPointer EvaluationPath { get; set; }
	public JsonNode? LocalInstance { get; set; }
	public EvaluationOptions Options { get; internal set; }
	public DynamicScope DynamicScope { get; }
	public Vocabulary[] Vocabularies { get; private set; }

	internal Uri? RefUri { get; set; }

	public EvaluationContext()
	{
		DynamicScope = new();
	}

	public EvaluationResults Evaluate(JsonNode? localSchema)
	{
		if (localSchema is JsonValue value)
		{
			var boolSchema = value.GetBool();
			if (!boolSchema.HasValue)
				throw new SchemaValidationException("Schema must be an object or a boolean", this);

			return new EvaluationResults
			{
				Valid = boolSchema.Value,
				InstanceLocation = InstanceLocation,
				EvaluationPath = EvaluationPath
			};
		}

		if (localSchema is not JsonObject objSchema)
			throw new SchemaValidationException("Schema must be an object or a boolean", this);

		var currentBaseUri = BaseUri;

		Uri? baseUri = null;
		if (objSchema.TryGetValue("$id", out var idNode, out _))
		{
			var id = (idNode as JsonValue)?.GetString();
			if (!Uri.TryCreate(id, UriKind.RelativeOrAbsolute, out baseUri))
				throw new SchemaValidationException("$id must be a valid URI", this);
			if (baseUri.IsAbsoluteUri && !string.IsNullOrEmpty(baseUri.Fragment))
				throw new SchemaValidationException("$id must not contain a fragment", this);
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
		if (RefUri is not null)
			BaseUri = RefUri;

		if (currentBaseUri != BaseUri) 
			DynamicScope.Push(BaseUri!);

		Vocabulary[] vocabs = [];
		var resourceRoot = Options.SchemaRegistry.Get(BaseUri!);
		if (resourceRoot.TryGetValue("$schema", out var schemaNode, out _))
		{
			var metaSchemaId = (schemaNode as JsonValue)?.GetString();
			if (metaSchemaId is null || !Uri.TryCreate(metaSchemaId, UriKind.Absolute, out var metaSchemaUri))
				throw new SchemaValidationException("$schema must be a valid URI", this);
			var metaSchema = Options.SchemaRegistry.Get(metaSchemaUri);
			if (metaSchema.TryGetValue("$vocabulary", out var vocabNode, out _))
			{
				if (vocabNode is not JsonObject vocabObject)
					throw new SchemaValidationException("$vocabulary must be an object", this);
				var vocabIds = vocabObject.ToDictionary(x => new Uri(x.Key, UriKind.Absolute), x => (x.Value as JsonValue)?.GetBool());
				if (vocabIds.Any(x => x.Value is null))
					throw new SchemaValidationException("$vocabulary values must be booleans", this);
				vocabs = vocabIds
					.Select(x => x.Value == true
						? FunctionalJsonSchema.Vocabularies.Get(x.Key)
						: FunctionalJsonSchema.Vocabularies.TryGet(x.Key))
					.Where(x => x is not null)
					.ToArray()!;
			}
		}

		var withHandlers = KeywordRegistry.GetHandlers(objSchema, vocabs);

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
			if (!ReferenceEquals(keywordResult, KeywordEvaluation.Skip))
				keywordResult.Key = entry.Keyword.Key;
			evaluations.Add(keywordResult);
			if (keywordResult.HasAnnotation)
				annotations[entry.Keyword.Key] = keywordResult.Annotation;
		}

		if (currentBaseUri != BaseUri)
			DynamicScope.Pop();

		return new EvaluationResults
		{
			Valid = valid,
			SchemaLocation = SchemaLocation.Segments.Any() 
				? new Uri(BaseUri!, SchemaLocation.ToString(JsonPointerStyle.UriEncoded))
				: BaseUri!,
			InstanceLocation = InstanceLocation,
			EvaluationPath = EvaluationPath,
			Details = evaluations.Any() ? evaluations.SelectMany(x => x.Children).ToArray() : null,
			Annotations = valid && annotations.Any() ? annotations : null
		};
	}
}