﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Yaml2JsonNode;

/// <summary>
/// Provides extensions to convert YAML models to JSON models.
/// </summary>
public static class YamlConverter
{
	/// <summary>
	/// Converts all of the documents in a YAML stream to <see cref="JsonNode"/>s.
	/// </summary>
	/// <param name="yaml">The YAML stream.</param>
	/// <returns>A collection of nodes representing the YAML documents in the stream.</returns>
	public static IEnumerable<JsonNode?> ToJsonNode(this YamlStream yaml)
	{
		return yaml.Documents.Select(x => x.ToJsonNode());
	}

	/// <summary>
	/// Converts a single YAML document to a <see cref="JsonNode"/>.
	/// </summary>
	/// <param name="yaml">The YAML document.</param>
	/// <returns>A `JsonNode` representative of the YAML document.</returns>
	public static JsonNode? ToJsonNode(this YamlDocument yaml)
	{
		return yaml.RootNode.ToJsonNode();
	}

	/// <summary>
	/// Converts a single YAML node to a <see cref="JsonNode"/>.
	/// </summary>
	/// <param name="yaml">The YAML node.</param>
	/// <returns>A `JsonNode` representative of the YAML node.</returns>
	/// <exception cref="NotSupportedException">Thrown for YAML that is not compatible with JSON.</exception>
	public static JsonNode? ToJsonNode(this YamlNode yaml)
	{
		return yaml switch
		{
			YamlMappingNode map => map.ToJsonObject(),
			YamlSequenceNode seq => seq.ToJsonArray(),
			YamlScalarNode scalar => scalar.ToJsonValue(),
			_ => throw new NotSupportedException("This yaml isn't convertible to JSON")
		};
	}

	/// <summary>
	/// Converts a single JSON node to a <see cref="YamlNode"/>.
	/// </summary>
	/// <param name="json"></param>
	/// <returns></returns>
	/// <exception cref="NotSupportedException"></exception>
	public static YamlNode ToYamlNode(this JsonNode json)
	{
		return json switch
		{
			null => new YamlScalarNode("null"),
			JsonObject obj => obj.ToYamlMapping(),
			JsonArray arr => arr.ToYamlSequence(),
			JsonValue val => val.ToYamlScalar(),
			_ => throw new NotSupportedException("This isn't a supported JsonNode")
			{
				Data =
				{
					["node"] = json,
					["type"] = json.GetType()
				}
			}
		};
	}

	private static JsonObject ToJsonObject(this YamlMappingNode yaml)
	{
		var node = new JsonObject();
		foreach (var keyValuePair in yaml)
		{
			var key = ((YamlScalarNode)keyValuePair.Key).Value!;
			node[key] = keyValuePair.Value.ToJsonNode();
		}

		return node;
	}

	private static YamlMappingNode ToYamlMapping(this JsonObject obj)
	{
		return new YamlMappingNode(obj.ToDictionary(x => (YamlNode)new YamlScalarNode(x.Key), x => x.Value!.ToYamlNode()));
	}

	private static JsonArray ToJsonArray(this YamlSequenceNode yaml)
	{
		var node = new JsonArray();
		foreach (var value in yaml)
		{
			node.Add(value.ToJsonNode());
		}

		return node;
	}

	private static YamlSequenceNode ToYamlSequence(this JsonArray arr)
	{
		return new YamlSequenceNode(arr.Select(x => x!.ToYamlNode()));
	}

	private static JsonValue? ToJsonValue(this YamlScalarNode yaml)
	{
		return yaml.Style switch
		{
			ScalarStyle.Plain => decimal.TryParse(yaml.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)
				? JsonValue.Create(d)
				: bool.TryParse(yaml.Value, out var b)
					? JsonValue.Create(b)
					: IsNull(yaml)
						? null
						: JsonValue.Create(yaml.Value)!,
			ScalarStyle.SingleQuoted => JsonValue.Create(yaml.Value)!,
			ScalarStyle.DoubleQuoted => JsonValue.Create(yaml.Value)!,
			ScalarStyle.Literal => JsonValue.Create(yaml.Value)!,
			ScalarStyle.Folded => JsonValue.Create(yaml.Value)!,
			ScalarStyle.Any => JsonValue.Create(yaml.Value)!,
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	// see https://yaml.org/type/null.html
	private static bool IsNull(YamlScalarNode yaml)
	{
		return yaml is
		{
			Style: ScalarStyle.Plain,
			Value: "" or "~" or "null" or "Null" or "NULL"
		};
	}

	private static YamlScalarNode ToYamlScalar(this JsonValue val)
	{
		if (val.TryGetValue(out string? s))
			return new YamlScalarNode(s)
			{
				Style = s.Contains('"') ? ScalarStyle.SingleQuoted : ScalarStyle.DoubleQuoted
			};

		return new YamlScalarNode(val.ToJsonString());
	}
}