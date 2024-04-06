using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Json.More;
using Json.Pointer;

namespace FunctionalJsonSchema;

public class RefKeywordHandler : IKeywordHandler
{
	public static RefKeywordHandler Instance { get; } = new();

	private static readonly Regex _anchorPattern202012 = new("^[A-Za-z_][-A-Za-z0-9._]*$");

	public string Name => "$ref";
	public string[]? Dependencies { get; }

	private RefKeywordHandler() { }

	public KeywordEvaluation Handle(JsonNode? keywordValue, EvaluationContext context, IReadOnlyCollection<KeywordEvaluation> evaluations)
	{
		var reference = (keywordValue as JsonValue)?.GetString();
		if (reference is null || !Uri.TryCreate(reference, UriKind.RelativeOrAbsolute, out _))
			throw new SchemaValidationException("$ref must contain a valid URI reference", context);

		var newUri = new Uri(context.BaseUri, reference);
		var fragment = newUri.Fragment;

		// TODO handle navigation loops

		var newBaseUri = new Uri(newUri.GetLeftPart(UriPartial.Query));

		JsonNode? target;
		if (JsonPointer.TryParse(fragment, out var pointer))
		{
			var targetBase = context.Options.SchemaRegistry.Get(newBaseUri);
			if (!pointer.TryEvaluate(targetBase, out target))
				throw new RefResolutionException(newBaseUri, pointer);
		}
		else
		{
			var anchor = fragment[1..];
			if (!_anchorPattern202012.IsMatch(anchor))
				throw new SchemaValidationException($"Unrecognized fragment type `{newUri}`", context);

			target = context.Options.SchemaRegistry.Get(newBaseUri, anchor);
		}

		var localContext = context;
		if (newBaseUri.OriginalString != context.BaseUri.OriginalString)
			localContext.RefUri = newBaseUri;
		localContext.EvaluationPath = localContext.EvaluationPath.Combine(Name);
		localContext.SchemaLocation = localContext.SchemaLocation.Combine(Name);

		var result = localContext.Evaluate(target);

		return new KeywordEvaluation
		{
			Valid = result.Valid,
			Children = [result]
		};
	}

	JsonNode?[] IKeywordHandler.GetSubschemas(JsonNode? keywordValue) => [];
}