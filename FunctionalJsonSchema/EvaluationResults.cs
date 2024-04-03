using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Json.Pointer;

namespace FunctionalJsonSchema;

public class EvaluationResults
{
	public bool Valid { get; set; }
	public JsonPointer SchemaLocation { get; set; }
	public JsonPointer InstanceLocation { get; set; }
	public JsonPointer EvaluationPath { get; set; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public EvaluationResults[]? Details { get; set; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public JsonObject? Annotations { get; set; }
}