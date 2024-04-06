using System;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public static class MetaSchemas
{
	public static readonly Uri Draft6Id = new("http://json-schema.org/draft-06/schema#");
	public static readonly JsonObject Draft6;

	public static readonly Uri Draft7Id = new("http://json-schema.org/draft-07/schema#");
	public static readonly JsonObject Draft7;

	public static readonly Uri Draft201909Id = new("https://json-schema.org/draft/2019-09/schema");
	public static readonly JsonObject Applicator201909;
	public static readonly JsonObject Content201909;
	public static readonly JsonObject Core201909;
	public static readonly JsonObject Format201909;
	public static readonly JsonObject Metadata201909;
	public static readonly JsonObject Draft201909;
	public static readonly JsonObject Validation201909;

	public static readonly Uri Draft202012Id = new("https://json-schema.org/draft/2020-12/schema");
	public static readonly JsonObject Applicator202012;
	public static readonly JsonObject Content202012;
	public static readonly JsonObject Core202012;
	public static readonly JsonObject FormatAnnotation202012;
	public static readonly JsonObject FormatAssertion202012;
	public static readonly JsonObject Metadata202012;
	public static readonly JsonObject Draft202012;
	public static readonly JsonObject Unevaluated202012;
	public static readonly JsonObject Validation202012;

	public static readonly Uri DraftNextId = new("https://json-schema.org/draft/next/schema");
	public static readonly JsonObject ApplicatorNext;
	public static readonly JsonObject ContentNext;
	public static readonly JsonObject CoreNext;
	public static readonly JsonObject FormatAnnotationNext;
	public static readonly JsonObject FormatAssertionNext;
	public static readonly JsonObject MetadataNext;
	public static readonly JsonObject DraftNext;
	public static readonly JsonObject UnevaluatedNext;
	public static readonly JsonObject ValidationNext;

	static MetaSchemas()
	{
		Draft6 = Register("FunctionalJsonSchema.schema06.json");

		Draft7 = Register("FunctionalJsonSchema.schema07.json");

		Applicator201909 = Register("FunctionalJsonSchema._2019_09.applicator.json");
		Content201909 = Register("FunctionalJsonSchema._2019_09.content.json");
		Core201909 = Register("FunctionalJsonSchema._2019_09.core.json");
		Format201909 = Register("FunctionalJsonSchema._2019_09.format.json");
		Metadata201909 = Register("FunctionalJsonSchema._2019_09.meta-data.json");
		Draft201909 = Register("FunctionalJsonSchema._2019_09.schema.json");
		Validation201909 = Register("FunctionalJsonSchema._2019_09.validation.json");

		Applicator202012 = Register("FunctionalJsonSchema._2020_12.applicator.json");
		Content202012 = Register("FunctionalJsonSchema._2020_12.content.json");
		Core202012 = Register("FunctionalJsonSchema._2020_12.core.json");
		FormatAnnotation202012 = Register("FunctionalJsonSchema._2020_12.format-annotation.json");
		FormatAssertion202012 = Register("FunctionalJsonSchema._2020_12.format-assertion.json");
		Metadata202012 = Register("FunctionalJsonSchema._2020_12.meta-data.json");
		Draft202012 = Register("FunctionalJsonSchema._2020_12.schema.json");
		Unevaluated202012 = Register("FunctionalJsonSchema._2020_12.unevaluated.json");
		Validation202012 = Register("FunctionalJsonSchema._2020_12.validation.json");

		ApplicatorNext = Register("FunctionalJsonSchema.Next.applicator.json");
		ContentNext = Register("FunctionalJsonSchema.Next.content.json");
		CoreNext = Register("FunctionalJsonSchema.Next.core.json");
		FormatAnnotationNext = Register("FunctionalJsonSchema.Next.format-annotation.json");
		FormatAssertionNext = Register("FunctionalJsonSchema.Next.format-assertion.json");
		MetadataNext = Register("FunctionalJsonSchema.Next.meta-data.json");
		DraftNext = Register("FunctionalJsonSchema.Next.schema.json");
		UnevaluatedNext = Register("FunctionalJsonSchema.Next.unevaluated.json");
		ValidationNext = Register("FunctionalJsonSchema.Next.validation.json");
	}

	private static JsonObject Register(string resourceName)
	{
		var schema = Load(resourceName);
		SchemaRegistry.Global.Register(schema);

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