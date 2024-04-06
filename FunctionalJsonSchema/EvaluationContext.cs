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
	public EvaluationOptions Options { get; internal init; }
	public DynamicScope DynamicScope { get; }

	internal Uri? RefUri { get; set; }
	internal Uri EvaluatingAs { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public EvaluationContext()
	{
		DynamicScope = new();
	}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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

		var lookup = Options.SchemaRegistry.GetUri(objSchema);
		if (lookup is not null)
			BaseUri = lookup;
		else if (RefUri is not null)
			BaseUri = RefUri;

		if (currentBaseUri != BaseUri) 
			DynamicScope.Push(BaseUri);

		EvaluatingAs ??= Options.DefaultMetaSchema;
		Vocabulary[] vocabs = [];
		var resourceRoot = Options.SchemaRegistry.Get(BaseUri);
		if (resourceRoot.TryGetValue("$schema", out var schemaNode, out _))
		{
			var metaSchemaId = (schemaNode as JsonValue)?.GetString();
			if (metaSchemaId is null || !Uri.TryCreate(metaSchemaId, UriKind.Absolute, out var metaSchemaUri))
				throw new SchemaValidationException("$schema must be a valid URI", this);

			EvaluatingAs = metaSchemaUri;
		}

		var metaSchema = Options.SchemaRegistry.Get(EvaluatingAs);
		if (metaSchema.TryGetValue("$vocabulary", out var vocabNode, out _))
		{
			// TODO: cache this
			if (vocabNode is not JsonObject vocabObject)
				throw new SchemaValidationException("$vocabulary must be an object", this);

			var vocabIds = vocabObject.ToDictionary(x => new Uri(x.Key, UriKind.Absolute), x => (x.Value as JsonValue)?.GetBool());
			if (vocabIds.Any(x => x.Value is null))
				throw new SchemaValidationException("$vocabulary values must be booleans", this);

			vocabs = vocabIds
				.Select(x => x.Value == true
					? Vocabularies.Get(x.Key)
					: Vocabularies.TryGet(x.Key))
				.Where(x => x is not null)
				.ToArray()!;
		}

		IEnumerable<(KeyValuePair<string, JsonNode?> Keyword, IKeywordHandler? Handler)> withHandlers;
		if (objSchema.ContainsKey("$ref") &&
		    (EvaluatingAs == MetaSchemas.Draft6Id || EvaluatingAs == MetaSchemas.Draft7Id))
		{
			if (currentBaseUri is not null && objSchema.ContainsKey("$id"))
				BaseUri = currentBaseUri;
			withHandlers = [(objSchema.Single(x => x.Key == "$ref"), RefKeywordHandler.Instance)];
		}
		else
			withHandlers = KeywordRegistry.GetHandlers(objSchema, vocabs);

		var valid = true;
		var evaluations = new List<KeywordEvaluation>();
		var annotations = new Dictionary<string, JsonNode?>();
		foreach (var entry in withHandlers)
		{
			var keywordContext = this;
			keywordContext.RefUri = null;
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