using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Json.Pointer;

namespace FunctionalJsonSchema;

public class EvaluationResults
{
	[JsonPropertyName("valid")]
	public bool Valid { get; set; }
	[JsonPropertyName("schemaLocation")]
	public JsonPointer SchemaLocation { get; set; }
	[JsonPropertyName("instanceLocation")]
	public JsonPointer InstanceLocation { get; set; }
	[JsonPropertyName("evaluationPath")]
	public JsonPointer EvaluationPath { get; set; }
	[JsonPropertyName("details")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public EvaluationResults[]? Details { get; set; }
	[JsonPropertyName("annotations")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public Dictionary<string, JsonNode?>? Annotations { get; set; }
}