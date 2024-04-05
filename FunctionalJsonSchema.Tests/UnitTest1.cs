using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema.Tests;

public class Tests
{
	private readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

	private static readonly JsonNode _schema = JsonNode.Parse(
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
		""")!;

	private static readonly JsonNode _instance = JsonNode.Parse(
		"""
		{"foo":1,"vroom":2}
		""")!;

	[Test]
	public void Test1()
	{
		var result = JsonSchema.Evaluate(_schema, _instance);
	}

	[Test]
	public void Test100()
	{
		for (int i = 0; i < 100; i++)
		{
			var result = JsonSchema.Evaluate(_schema, _instance);
		}
	}
}