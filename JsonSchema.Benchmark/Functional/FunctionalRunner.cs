using System.Text.Json.Nodes;
using BenchmarkDotNet.Attributes;

using FJsonSchema = FunctionalJsonSchema.JsonSchema;

namespace Json.Schema.Benchmark.Functional;

[MemoryDiagnoser]
public class FunctionalRunner
{
	private const string _schemaText =
		"""
		{
		  "$schema": "https://json-schema.org/draft/2020-12/schema",
		  "properties": {
		    "foo": {},
		    "bar": {}
		  },
		  "patternProperties": {
		    "^v": {}
		  },
		  "additionalProperties": false
		}
		""";

	private static readonly JsonNode _instance = JsonNode.Parse(
		"""
		{"foo":1,"vroom":2}
		""")!;

	[Params(1,10)]
	public int Count { get; set; }

	[Benchmark]
	public int OOP()
	{
		for (int i = 0; i < Count; i++)
		{
			var schema = JsonSchema.FromText(_schemaText);

			_ = schema.Evaluate(_instance);
		}

		return Count;
	}

	[Benchmark]
	public int OOP_Reuse()
	{
		var schema = JsonSchema.FromText(_schemaText);

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
			var schema = JsonNode.Parse(_schemaText)!;

			_ = FJsonSchema.Evaluate(schema, _instance);
		}

		return Count;
	}

	[Benchmark]
	public int Functional_Reuse()
	{
		var schema = JsonNode.Parse(_schemaText)!;

		for (int i = 0; i < Count; i++)
		{
			_ = FJsonSchema.Evaluate(schema, _instance);
		}

		return Count;
	}
}