using System;
using System.Text.Json.Nodes;
using Json.Pointer;

namespace FunctionalJsonSchema;

public struct EvaluationContext
{
	public Uri BaseUri { get; set; }
	public JsonPointer SchemaLocation { get; set; }
	public JsonPointer InstanceLocation { get; set; }
	public JsonPointer EvaluationPath { get; set; }
	public JsonNode? LocalInstance { get; set; }
	public EvaluationResults Results { get; set; }
}