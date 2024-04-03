using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public static class KeywordRegistry
{
	private static readonly Dictionary<string, IKeywordHandler> _handlers =
		typeof(IKeywordHandler)
			.Assembly
			.DefinedTypes
			.Where(x => typeof(IKeywordHandler).IsAssignableFrom(x) && !x.IsAbstract)
			.Select(x => (IKeywordHandler)Activator.CreateInstance(x))
			.ToDictionary(x => x.Name);

	public static IEnumerable<(KeyValuePair<string, JsonNode?> Keyword, IKeywordHandler? Handler)> GetHandlers(JsonObject schema)
	{
		foreach (var kvp in schema)
		{
			yield return (kvp, _handlers.GetValueOrDefault(kvp.Key));
		}
	}
}