using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema.Tests
{
	public class Tests
	{
		private readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

		[Test]
		public void Test1()
		{
			var schema = new JsonObject
			{
				["type"] = "object",
				["properties"] = new JsonObject
				{
					["foo"] = new JsonObject
					{
						["type"] = "string"
					}
				}
			};

			var instance = new JsonObject
			{
				["foo"] = "value"
			};

			var result = JsonSchema.Evaluate(schema, instance);

			Console.WriteLine(JsonSerializer.Serialize(result, _serializerOptions));
			Assert.IsTrue(result.Valid);
		}
	}
}