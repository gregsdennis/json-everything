using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class KeywordEvaluation
{
	public static KeywordEvaluation Skip { get; } = new() { Valid = true };

	public string Key { get; internal set; }
	public bool Valid { get; set; }
	public EvaluationResults[] Children { get; set; } = [];
	public bool HasAnnotation { get; set; }
	public JsonNode? Annotation { get; set; }

	public static implicit operator KeywordEvaluation(bool valid) => new() { Valid = valid };
}

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
			if (evaluation.Key != keyword) continue;
			if (!evaluation.HasAnnotation) continue;
			if (evaluation.Annotation is not T node) continue;

			annotations.Add(node);
			annotations.AddRange(evaluation.Children.GetAllAnnotations<T>(keyword));
		}

		return [.. annotations];
	}
}

public static class EvaluationResultsExtensions
{
	public static IEnumerable<T> GetAllAnnotations<T>(this IEnumerable<EvaluationResults> results, string keyword)
		where T : JsonNode
	{
		var toCheck = new Queue<EvaluationResults>(results);

		while (toCheck.Any())
		{
			var current = toCheck.Dequeue();

			if (current.Annotations is not null)
			{
				if (current.Annotations.TryGetPropertyValue(keyword, out var node) && node is T annotation)
					yield return annotation;
			}

			if (current.Details is not null)
			{
				foreach (var detail in current.Details)
				{
					toCheck.Enqueue(detail);
				}
			}
		}
	}
}