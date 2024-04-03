﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public static class KeywordEvaluationExtensions
{
	public static bool TryGetAnnotation<T>(this IEnumerable<KeywordEvaluation> evaluations, string keyword, [NotNullWhen(true)] out T? annotation)
		where T : JsonNode
	{
		var evaluation = evaluations.SingleOrDefault(x => x.Key == keyword);
		if (evaluation is null || !evaluation.HasAnnotation || evaluation.Annotation is not T node)
		{
			annotation = null;
			return false;
		}

		annotation = node;
		return true;
	}

	public static T[] GetAllAnnotations<T>(this IEnumerable<KeywordEvaluation> evaluations, string keyword)
		where T : JsonNode
	{
		var annotations = new List<T>();

		foreach (var evaluation in evaluations)
		{
			if (evaluation.Key == keyword &&
			    evaluation is { HasAnnotation: true, Annotation: T node })
				annotations.Add(node);

			annotations.AddRange(evaluation.Children.GetAllAnnotations<T>(keyword));
		}

		return [.. annotations];
	}
}