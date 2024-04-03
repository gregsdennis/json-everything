using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;

using FJsonSchema = FunctionalJsonSchema.JsonSchema;

namespace Json.Schema.Benchmark.Functional;

[MemoryDiagnoser]
public class FunctionalRunner
{
	private static readonly JsonNode _instance = new JsonObject
	{
		["foo"] = "value"
	};

	[Params(1,100)]
	public int Count { get; set; }

	[Benchmark]
	public int OOP()
	{
		for (int i = 0; i < Count; i++)
		{
			var schema = JsonSchema.FromText(
				"""
				{
				  "type": "object",
				  "properties": {
				    "foo": { "type": "string" }
				  }
				}
				""");

			_ = schema.Evaluate(_instance);
		}

		return Count;
	}

	[Benchmark]
	public int OOP_Reuse()
	{
		var schema = JsonSchema.FromText(
			"""
			{
			  "type": "object",
			  "properties": {
			    "foo": { "type": "string" }
			  }
			}
			""");

		for (int i = 0; i < Count; i++)
		{
			_ = schema.Evaluate(_instance);
		}

		return Count;
	}

	[Benchmark]
	public int Functional()
	{
		for (int i = 0; i < Count; i++)
		{
			var schema = JsonNode.Parse(
				"""
				{
				  "type": "object",
				  "properties": {
				    "foo": { "type": "string" }
				  }
				}
				""")!;

			_ = FJsonSchema.Evaluate(schema, _instance);
		}

		return Count;
	}

	[Benchmark]
	public int Functional_Reuse()
	{
		var schema = JsonNode.Parse(
			"""
			{
			  "type": "object",
			  "properties": {
			    "foo": { "type": "string" }
			  }
			}
			""")!;

		for (int i = 0; i < Count; i++)
		{
			_ = FJsonSchema.Evaluate(schema, _instance);
		}

		return Count;
	}
}