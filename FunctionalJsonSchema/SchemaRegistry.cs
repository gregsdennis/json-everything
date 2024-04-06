﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace FunctionalJsonSchema;

public class SchemaRegistry
{
	private class Registration
	{
		public required JsonObject Root { get; init; }
		public Dictionary<string, JsonObject> Anchors { get; } = [];
		public Dictionary<string, JsonObject> DynamicAnchors { get; } = [];
		public JsonObject? RecursiveAnchor { get; set; }
	}

	private readonly Dictionary<Uri, Registration> _registry;
	private readonly Dictionary<JsonObject, Uri> _reverseLookup;

	internal SchemaRegistry(SchemaRegistry? other)
	{
		_registry = new(other?._registry ?? []);
		_reverseLookup = [];
	}

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
			_reverseLookup[registration.Value.Root] = registration.Key;
		}

		if (!_registry.ContainsKey(baseUri))
		{
			// schema contains different $id
			var registration = _registry.First(x => ReferenceEquals(x.Value.Root, schema)).Value;
			_registry[baseUri] = registration;
		}
	}

	internal JsonObject Get(Uri baseUri, string? anchor = null)
	{
		return GetAnchor(baseUri, anchor, false) ?? throw new RefResolutionException(baseUri, anchor);
	}

	internal Uri? GetUri(JsonObject schema)
	{
		return _reverseLookup.GetValueOrDefault(schema);
	}

	internal (JsonObject, Uri) Get(DynamicScope scope, Uri baseUri, string anchor, bool requireLocalAnchor)
	{
		if (requireLocalAnchor)
		{
			var registration = _registry[baseUri];
			if (!registration.DynamicAnchors.ContainsKey(anchor))
			{
				var target = GetAnchor(baseUri, anchor, false) ?? throw new RefResolutionException(baseUri, anchor);
				return (target, baseUri);
			}
		}

		foreach (var uri in scope.Reverse())
		{
			var target = GetAnchor(uri, anchor, true);
			if (target is not null) return (target, uri);
		}

		throw new RefResolutionException(scope.LocalScope, anchor, true);
	}

	internal (JsonObject, Uri) GetRecursive(DynamicScope scope)
	{
		(JsonObject, Uri)? resolved = null;
		foreach (var uri in scope)
		{
			var registration = _registry[uri];
			if (registration.RecursiveAnchor is null)
				return resolved ?? (registration.Root, uri);

			resolved = (registration.RecursiveAnchor, uri);
		}

		// resolved should always be set
		return resolved ?? throw new NotImplementedException();
	}

	private JsonObject? GetAnchor(Uri baseUri, string? anchor, bool isDynamic)
	{
		if (!_registry.TryGetValue(baseUri, out var registration)) return null;

		if (anchor is null) return registration.Root;

		var anchorList = isDynamic ? registration.DynamicAnchors : registration.Anchors;

		return anchorList.GetValueOrDefault(anchor);
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

			var dynamicAnchor = (currentSchema["$dynamicAnchor"] as JsonValue)?.GetString();
			if (dynamicAnchor is not null)
			{
				registration.Anchors[dynamicAnchor] = currentSchema;
				registration.DynamicAnchors[dynamicAnchor] = currentSchema;
			}

			var recursiveAnchor = (currentSchema["$recursiveAnchor"] as JsonValue)?.GetBool() == true;
			if (recursiveAnchor) 
				registration.RecursiveAnchor = currentSchema;

			var anchor = (currentSchema["$anchor"] as JsonValue)?.GetString();
			if (anchor is not null)
				registration.Anchors[anchor] = currentSchema;

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
