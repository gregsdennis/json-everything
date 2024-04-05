using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public static class KeywordRegistry
{
	private static readonly Dictionary<string, IKeywordHandler> _handlers;
	private static readonly Dictionary<string, int> _keywordPriorities;

	static KeywordRegistry()
	{
		_handlers = new IKeywordHandler[]
		{
			AdditionalPropertiesKeywordHandler.Instance,
			AllOfKeywordHandler.Instance,
			AnchorKeywordHandler.Instance,
			AnyOfKeywordHandler.Instance,
			CommentKeywordHandler.Instance,
			ConstKeywordHandler.Instance,
			ContainsKeywordHandler.Instance,
			ContentEncodingKeywordHandler.Instance,
			ContentMediaTypeKeywordHandler.Instance,
			ContentSchemaKeywordHandler.Instance,
			DefaultKeywordHandler.Instance,
			DefsKeywordHandler.Instance,
			DependentRequiredKeywordHandler.Instance,
			DependentSchemasKeywordHandler.Instance,
			DescriptionKeywordHandler.Instance,
			DynamicAnchorKeywordHandler.Instance,
			DynamicRefKeywordHandler.Instance,
			ElseKeywordHandler.Instance,
			EnumKeywordHandler.Instance,
			ExamplesKeywordHandler.Instance,
			ExclusiveMaximumKeywordHandler.Instance,
			ExclusiveMinimumKeywordHandler.Instance,
			FormatKeywordHandler.Annotate,
			IdKeywordHandler.Instance,
			IfKeywordHandler.Instance,
			ItemsKeywordHandler.Instance,
			MaxContainsKeywordHandler.Instance,
			MaximumKeywordHandler.Instance,
			MaxItemsKeywordHandler.Instance,
			MaxPropertiesKeywordHandler.Instance,
			MinContainsKeywordHandler.Instance,
			MinimumKeywordHandler.Instance,
			MinItemsKeywordHandler.Instance,
			MinPropertiesKeywordHandler.Instance,
			MultipleOfKeywordHandler.Instance,
			NotKeywordHandler.Instance,
			OneOfKeywordHandler.Instance,
			PatternKeywordHandler.Instance,
			PatternPropertiesKeywordHandler.Instance,
			PrefixItemsKeywordHandler.Instance,
			PropertiesKeywordHandler.Instance,
			PropertyNamesKeywordHandler.Instance,
			ReadOnlyKeywordHandler.Instance,
			RefKeywordHandler.Instance,
			RequiredKeywordHandler.Instance,
			SchemaKeywordHandler.Instance,
			ThenKeywordHandler.Instance,
			TitleKeywordHandler.Instance,
			TypeKeywordHandler.Instance,
			UnevaluatedItemsKeywordHandler.Instance,
			UnevaluatedPropertiesKeywordHandler.Instance,
			UniqueItemsKeywordHandler.Instance,
			VocabularyKeywordHandler.Instance,
			WriteOnlyKeywordHandler.Instance
		}.ToDictionary(x => x.Name);

		_keywordPriorities = [];
		UpdatePriorities();
	}

	public static IKeywordHandler? Get(string name) => _handlers.GetValueOrDefault(name);

	public static void Register(IKeywordHandler handler)
	{
		_handlers[handler.Name] = handler;
		UpdatePriorities();
	}

	internal static void Register(Vocabulary[] vocabularies)
	{
		var handlers = vocabularies.SelectMany(x => x.Handlers);
		foreach (var handler in handlers)
		{
			_handlers[handler.Name] = handler;
		}
		UpdatePriorities();
	}

	internal static IEnumerable<(KeyValuePair<string, JsonNode?> Keyword, IKeywordHandler? Handler)> GetHandlers(JsonObject schema, Vocabulary[] vocabs)
	{
		var pairs = vocabs.Length != 0
			? schema.Join(vocabs.SelectMany(x => x.Handlers),
				s => s.Key,
				h => h.Name,
				(s, h) => (Keyword: s, Handler: h))!
			: schema.Select(kvp => (Keyword: kvp, Handler: _handlers.GetValueOrDefault(kvp.Key)));

		return pairs.OrderBy(x => _keywordPriorities.GetValueOrDefault(x.Keyword.Key));
	}

	private static void UpdatePriorities()
	{
		_keywordPriorities.Clear();

		_keywordPriorities["$schema"] = -2;
		_keywordPriorities["$id"] = -1;
		_keywordPriorities["unevaluatedItems"] = int.MaxValue;
		_keywordPriorities["unevaluatedProperties"] = int.MaxValue;

		var allKeywords = _handlers
			.Where(x => !_keywordPriorities.ContainsKey(x.Key))
			.Select(x => x.Value)
			.ToList();

		var priority = 1; // 0 for unhandled keywords (annotations)
		while (allKeywords.Count != 0)
		{
			var priorityKeywords = allKeywords
				.Where(x => x.Dependencies is null ||
				            x.Dependencies.All(d => _keywordPriorities.ContainsKey(d)))
				.ToArray();

			// without this, we loop forever
			if (!priorityKeywords.Any())
				throw new Exception($"Could not find handlers for: {string.Join(", ", allKeywords.SelectMany(x => x.Dependencies).Except(_keywordPriorities.Keys))}");

			foreach (var keyword in priorityKeywords)
			{
				_keywordPriorities[keyword.Name] = priority;
				allKeywords.Remove(keyword);
			}

			priority++;
		}
	}
}