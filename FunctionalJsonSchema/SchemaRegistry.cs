using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class SchemaRegistry
{
	private class Registration
	{
		public JsonObject Root { get; set; }
		public Dictionary<string, JsonObject> Anchors { get; } = [];
		public Dictionary<string, JsonObject> DynamicAnchors { get; } = [];
	}

	private readonly Dictionary<Uri, Registration> _registry = new();

	public Uri Register(JsonObject schema)
	{
		var idText = (schema["$id"] as JsonValue)?.GetString();

		var id = idText is null ? GenerateId() : new Uri(JsonSchema.DefaultBaseUri, idText);

		Register(id, schema);

		return id;
	}

	public void Register(Uri baseUri, JsonObject schema)
	{
		var registrations = Scan(baseUri, schema);

		foreach (var registration in registrations)
		{
			_registry[registration.Key] = registration.Value;
		}

		if (!_registry.ContainsKey(baseUri))
		{
			// schema contains different $id
			var registration = _registry.First(x => ReferenceEquals(x.Value.Root, schema)).Value;
			_registry[baseUri] = registration;
		}
	}

	public JsonObject Get(Uri baseUri, string? anchor = null, bool isDynamic = false)
	{
		if (!_registry.TryGetValue(baseUri, out var registration))
			throw new RefResolutionException(baseUri);

		if (anchor is null) return registration.Root;

		var anchorList = isDynamic ? registration.DynamicAnchors : registration.Anchors;

		if (!anchorList.TryGetValue(anchor, out var target))
			throw new RefResolutionException(baseUri, anchor, isDynamic);

		return target;
	}

	private static Uri GenerateId() => new(JsonSchema.DefaultBaseUri, Guid.NewGuid().ToString("N")[..10]);

	private static Dictionary<Uri, Registration> Scan(Uri baseUri, JsonObject schema)
	{
		var toCheck = new Queue<(Uri, JsonObject)>();
		toCheck.Enqueue((baseUri, schema));

		var registrations = new Dictionary<Uri, Registration>();

		while (toCheck.Any())
		{
			var (currentUri, currentSchema) = toCheck.Dequeue();

			var idText = (currentSchema["$id"] as JsonValue)?.GetString();
			if (idText is not null) 
				currentUri = new Uri(currentUri, idText);

			if (!registrations.TryGetValue(currentUri, out var registration))
				registrations[currentUri] = registration = new Registration
				{
					Root = currentSchema
				};

			var anchor = (currentSchema["$anchor"] as JsonValue)?.GetString();
			if (anchor is not null)
				registration.Anchors[anchor] = currentSchema;

			var dynamicAnchor = (currentSchema["$dynamicAnchor"] as JsonValue)?.GetString();
			if (dynamicAnchor is not null)
			{
				registration.Anchors[dynamicAnchor] = currentSchema;
				registration.DynamicAnchors[dynamicAnchor] = currentSchema;
			}

			foreach (var kvp in currentSchema)
			{
				var handler = KeywordRegistry.Get(kvp.Key);
				if (handler is null) continue;

				foreach (var subschema in handler.GetSubschemas(kvp.Value))
				{
					if (subschema is not JsonObject objSubschema) continue;

					toCheck.Enqueue((currentUri, objSubschema));
				}
			}
		}

		return registrations;
	}
}