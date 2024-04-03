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
		_handlers = typeof(IKeywordHandler)
			.Assembly
			.DefinedTypes
			.Where(x => typeof(IKeywordHandler).IsAssignableFrom(x) && !x.IsAbstract)
			.Select(x => (IKeywordHandler)Activator.CreateInstance(x))
			.ToDictionary(x => x.Name);

		_keywordPriorities = [];
		UpdatePriorities();
	}

	public static IEnumerable<(KeyValuePair<string, JsonNode?> Keyword, IKeywordHandler? Handler)> GetHandlers(JsonObject schema)
	{
		return schema
			.Select(kvp => (Keyword: kvp, Handler: _handlers.GetValueOrDefault(kvp.Key)))
			.OrderBy(x => _keywordPriorities.GetValueOrDefault(x.Keyword.Key));
	}

	private static void UpdatePriorities()
	{
		_keywordPriorities.Clear();

		_keywordPriorities["$schema"] = -2;
		_keywordPriorities["$id"] = -1;
		_keywordPriorities["unevaluatedItems"] = int.MaxValue;
		_keywordPriorities["unevaluatedProperties"] = int.MaxValue;
		var allKeywords = _handlers
			.Where(x => !_keywordPriorities.Keys.Contains(x.Key))
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