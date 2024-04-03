using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public class KeywordEvaluation
{
	public static KeywordEvaluation Skip { get; } = new() { Valid = true };

	public bool Valid { get; set; }
	public EvaluationResults[] Children { get; set; } = [];
	public bool HasAnnotation { get; set; }
	public JsonNode? Annotation { get; set; }
}