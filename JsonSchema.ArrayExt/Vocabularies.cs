﻿using System.Collections.Generic;
using System.Text.Json.Serialization;
using Json.Pointer;

namespace Json.Schema.ArrayExt;

/// <summary>
/// Declares the vocabularies of the supported drafts.
/// </summary>
public static class Vocabularies
{
	/// <summary>
	/// The array extensions vocabulary ID.
	/// </summary>
	public const string ArrayExtId = "https://docs.json-everything.net/schema/vocabs/array-ext";

	/// <summary>
	/// The array extensions vocabulary.
	/// </summary>
	public static readonly Vocabulary ArrayExt = new(ArrayExtId, typeof(UniqueKeysKeyword), typeof(OrderingKeyword));

	/// <summary>
	/// Registers the all components required to use the array extensions vocabulary.
	/// </summary>
	public static void Register(VocabularyRegistry? vocabRegistry = null, SchemaRegistry? schemaRegistry = null)
	{
		vocabRegistry ??= VocabularyRegistry.Global;
		schemaRegistry ??= SchemaRegistry.Global;

		vocabRegistry.Register(ArrayExt);
		SchemaKeywordRegistry.Register<UniqueKeysKeyword>(ArrayExtSerializerContext.Default);
		SchemaKeywordRegistry.Register<OrderingKeyword>(ArrayExtSerializerContext.Default);
		schemaRegistry.Register(MetaSchemas.ArrayExt);
		schemaRegistry.Register(MetaSchemas.ArrayExt_202012);
	}
}

[JsonSerializable(typeof(UniqueKeysKeyword))]
[JsonSerializable(typeof(OrderingKeyword))]
[JsonSerializable(typeof(IEnumerable<JsonPointer>))]
[JsonSerializable(typeof(List<JsonPointer>))]
[JsonSerializable(typeof(IEnumerable<OrderingSpecifier>))]
[JsonSerializable(typeof(List<OrderingSpecifier>))]
[JsonSerializable(typeof(int))]
internal partial class ArrayExtSerializerContext : JsonSerializerContext;
