using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public static class MetaSchemas
{
	public static readonly JsonObject Applicator202012;
	public static readonly JsonObject Content202012;
	public static readonly JsonObject Core202012;
	public static readonly JsonObject FormatAnnotation202012;
	public static readonly JsonObject FormatAssertion202012;
	public static readonly JsonObject Metadata202012;
	public static readonly JsonObject Draft202012;
	public static readonly JsonObject Unevaluated202012;
	public static readonly JsonObject Validation202012;

	internal static SchemaRegistry Registry { get; } = new(null);

	static MetaSchemas()
	{
		Register("FunctionalJsonSchema._2019_09.applicator.json");
		Register("FunctionalJsonSchema._2019_09.content.json");
		Register("FunctionalJsonSchema._2019_09.core.json");
		Register("FunctionalJsonSchema._2019_09.format.json");
		Register("FunctionalJsonSchema._2019_09.meta-data.json");
		Register("FunctionalJsonSchema._2019_09.schema.json");
		Register("FunctionalJsonSchema._2019_09.validation.json");
		Applicator202012 = Register("FunctionalJsonSchema._2020_12.applicator.json");
		Content202012 = Register("FunctionalJsonSchema._2020_12.content.json");
		Core202012 = Register("FunctionalJsonSchema._2020_12.core.json");
		FormatAnnotation202012 = Register("FunctionalJsonSchema._2020_12.format-annotation.json");
		FormatAssertion202012 = Register("FunctionalJsonSchema._2020_12.format-assertion.json");
		Metadata202012 = Register("FunctionalJsonSchema._2020_12.meta-data.json");
		Draft202012 = Register("FunctionalJsonSchema._2020_12.schema.json");
		Unevaluated202012 = Register("FunctionalJsonSchema._2020_12.unevaluated.json");
		Validation202012 = Register("FunctionalJsonSchema._2020_12.validation.json");
		Register("FunctionalJsonSchema.Next.applicator.json");
		Register("FunctionalJsonSchema.Next.content.json");
		Register("FunctionalJsonSchema.Next.core.json");
		Register("FunctionalJsonSchema.Next.format-annotation.json");
		Register("FunctionalJsonSchema.Next.format-assertion.json");
		Register("FunctionalJsonSchema.Next.meta-data.json");
		Register("FunctionalJsonSchema.Next.schema.json");
		Register("FunctionalJsonSchema.Next.unevaluated.json");
		Register("FunctionalJsonSchema.Next.validation.json");
		Register("FunctionalJsonSchema.OpenAPI._3._1.openapi-dialect-base.json");
		Register("FunctionalJsonSchema.OpenAPI._3._1.openapi-meta-base.json");
		Register("FunctionalJsonSchema.OpenAPI._3._1.schema.json");
		Register("FunctionalJsonSchema.schema06.json");
		Register("FunctionalJsonSchema.schema07.json");
	}

	private static JsonObject Register(string resourceName)
	{
		var schema = Load(resourceName);
		Registry.Register(schema);

		return schema;
	}

	private static JsonObject Load(string resourceName)
	{
		var resourceStream = typeof(MetaSchemas).Assembly.GetManifestResourceStream(resourceName);
		using var reader = new StreamReader(resourceStream!, Encoding.UTF8);

		var text = reader.ReadToEnd();
		return (JsonObject) JsonNode.Parse(text)!;
	}
}