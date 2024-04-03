using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace FunctionalJsonSchema;

public static class KeywordRegistry
{
	private static readonly Dictionary<string, IKeywordHandler> _handlers =
		new()
		{
			{"type", new TypeKeywordHandler()},
			{"properties", new PropertiesKeywordHandler()}
		};

	public static IEnumerable<(KeyValuePair<string, JsonNode?> Keyword, IKeywordHandler? Handler)> GetHandlers(JsonObject schema)
	{
		foreach (var kvp in schema)
		{
			yield return (kvp, _handlers.GetValueOrDefault(kvp.Key));
		}
	}
}