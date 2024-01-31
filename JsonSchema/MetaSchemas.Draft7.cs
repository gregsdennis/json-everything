﻿using System;
using System.Text.Json.Nodes;

namespace Json.Schema;

public static partial class MetaSchemas
{
	internal const string Draft7IdValue = "http://json-schema.org/draft-07/schema#";

	/// <summary>
	/// The Draft 7 Core meta-schema ID.
	/// </summary>
	public static readonly Uri Draft7Id = new(Draft7IdValue);

	/// <summary>
	/// The Draft 7 Core meta-schema.
	/// </summary>
	public static readonly JsonSchema Draft7 = new JsonSchemaBuilder()
		.Schema(Draft7Id)
		.Id(Draft7Id)
		.Title("Core schema meta-schema")
		.Definitions(
			("schemaArray", new JsonSchemaBuilder()
				.Type(SchemaValueType.Array)
				.MinItems(1)
				.Items(JsonSchemaBuilder.RefRoot())
			),
			("nonNegativeInteger", new JsonSchemaBuilder()
				.Type(SchemaValueType.Integer)
				.Minimum(0)
			),
			("nonNegativeIntegerDefault0", new JsonSchemaBuilder()
				.AllOf(
					new JsonSchemaBuilder().Ref("#/definitions/nonNegativeInteger"),
					new JsonSchemaBuilder().Default(0)
				)
			),
			("simpleTypes", new JsonSchemaBuilder()
				.Enum("array",
					"boolean",
					"integer",
					"null",
					"number",
					"object",
					"string")
			),
			("stringArray", new JsonSchemaBuilder()
				.Type(SchemaValueType.Array)
				.Items(new JsonSchemaBuilder().Type(SchemaValueType.String))
				.UniqueItems(true)
				.Default(new JsonArray())
			)
		)
		.Type(SchemaValueType.Object | SchemaValueType.Boolean)
		.Properties(
			(IdKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.String)
				.Format(Formats.UriReference)
			),
			(SchemaKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.String)
				.Format(Formats.Uri)
			),
			(RefKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.String)
				.Format(Formats.UriReference)
			),
			(CommentKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.String)
			),
			(TitleKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.String)
			),
			(DescriptionKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.String)
			),
			(DefaultKeyword.Name, true),
			(ReadOnlyKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.Boolean)
				.Default(false)
			),
			(WriteOnlyKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.Boolean)
				.Default(false)
			),
			(ExamplesKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.Array)
				.Items(true)
			),
			(MultipleOfKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.Number)
				.ExclusiveMinimum(0)
			),
			(MaximumKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.Number)
			),
			(ExclusiveMaximumKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.Number)
			),
			(MinimumKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.Number)
			),
			(ExclusiveMinimumKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.Number)
			),
			(MaxLengthKeyword.Name, new JsonSchemaBuilder()
				.Ref("#/definitions/nonNegativeInteger")
			),
			(MinLengthKeyword.Name, new JsonSchemaBuilder()
				.Ref("#/definitions/nonNegativeIntegerDefault0")
			),
			(PatternKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.String)
				.Format(Formats.Regex)
			),
			(AdditionalItemsKeyword.Name, JsonSchemaBuilder.RefRoot()),
			(ItemsKeyword.Name, new JsonSchemaBuilder()
				.AnyOf(
					JsonSchemaBuilder.RefRoot(),
					new JsonSchemaBuilder().Ref("#/definitions/schemaArray")
				)
				.Default(true)
			),
			(MaxItemsKeyword.Name, new JsonSchemaBuilder().Ref("#/definitions/nonNegativeInteger")),
			(MinItemsKeyword.Name, new JsonSchemaBuilder().Ref("#/definitions/nonNegativeIntegerDefault0")),
			(UniqueItemsKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.Boolean)
				.Default(false)
			),
			(ContainsKeyword.Name, JsonSchemaBuilder.RefRoot()),
			(MaxPropertiesKeyword.Name, new JsonSchemaBuilder().Ref("#/definitions/nonNegativeInteger")),
			(MinPropertiesKeyword.Name, new JsonSchemaBuilder().Ref("#/definitions/nonNegativeIntegerDefault0")),
			(RequiredKeyword.Name, new JsonSchemaBuilder().Ref("#/definitions/stringArray")),
			(AdditionalPropertiesKeyword.Name, JsonSchemaBuilder.RefRoot()),
			(DefinitionsKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.Object)
				.AdditionalProperties(JsonSchemaBuilder.RefRoot())
				.Default(new JsonObject())
			),
			(PropertiesKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.Object)
				.AdditionalProperties(JsonSchemaBuilder.RefRoot())
				.Default(new JsonObject())
			),
			(PatternPropertiesKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.Object)
				.AdditionalProperties(JsonSchemaBuilder.RefRoot())
				.PropertyNames(new JsonSchemaBuilder().Format(Formats.Regex))
				.Default(new JsonObject())
			),
			(DependenciesKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.Object)
				.AdditionalProperties(new JsonSchemaBuilder()
					.AnyOf(
						JsonSchemaBuilder.RefRoot(),
						new JsonSchemaBuilder().Ref("#/definitions/stringArray")
					)
				)
			),
			(PropertyNamesKeyword.Name, JsonSchemaBuilder.RefRoot()),
			(ConstKeyword.Name, true),
			(EnumKeyword.Name, new JsonSchemaBuilder()
				.Type(SchemaValueType.Array)
				.Items(true)
				.MinItems(1)
				.UniqueItems(true)
			),
			(TypeKeyword.Name, new JsonSchemaBuilder()
				.AnyOf(
					new JsonSchemaBuilder().Ref("#/definitions/simpleTypes"),
					new JsonSchemaBuilder()
						.Type(SchemaValueType.Array)
						.Items(new JsonSchemaBuilder().Ref("#/definitions/simpleTypes"))
						.MinItems(1)
						.UniqueItems(true)
				)
			),
			(FormatKeyword.Name, new JsonSchemaBuilder().Type(SchemaValueType.String)),
			(ContentMediaTypeKeyword.Name, new JsonSchemaBuilder().Type(SchemaValueType.String)),
			(ContentEncodingKeyword.Name, new JsonSchemaBuilder().Type(SchemaValueType.String)),
			(IfKeyword.Name, JsonSchemaBuilder.RefRoot()),
			(ThenKeyword.Name, JsonSchemaBuilder.RefRoot()),
			(ElseKeyword.Name, JsonSchemaBuilder.RefRoot()),
			(AllOfKeyword.Name, new JsonSchemaBuilder().Ref("#/definitions/schemaArray")),
			(AnyOfKeyword.Name, new JsonSchemaBuilder().Ref("#/definitions/schemaArray")),
			(OneOfKeyword.Name, new JsonSchemaBuilder().Ref("#/definitions/schemaArray")),
			(NotKeyword.Name, JsonSchemaBuilder.RefRoot())
		)
		.Default(true);
}