﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using Json.More;

namespace Json.Logic;

/// <summary>
/// Provides extended functionality for serializing rules.
/// </summary>
public static class JsonWriterExtensions
{
	/// <summary>
	/// Writes a rule to the stream, taking its specific type into account.
	/// </summary>
	/// <param name="writer">The writer.</param>
	/// <param name="rule">The rule.</param>
	/// <param name="options">Serializer options.</param>
	[RequiresDynamicCode("Calls JsonSerializer.Serialize. Make sure the options object contains all relevant JsonTypeInfos before suppressing this warning.")]
	[RequiresUnreferencedCode("Calls JsonSerializer.Serialize. Make sure the options object contains all relevant JsonTypeInfos before suppressing this warning.")]
	public static void WriteRule(this Utf8JsonWriter writer, Rule? rule, JsonSerializerOptions options)
	{
		if (rule == null)
		{
			writer.WriteNullValue();
			return;
		}

		options.Write(writer, rule, rule.GetType());
	}

	/// <summary>
	/// Writes a rule to the stream, taking its specific type into account.
	/// </summary>
	/// <param name="writer">The writer.</param>
	/// <param name="rules">The rules.</param>
	/// <param name="options">Serializer options.</param>
	/// <param name="unwrapSingle">Unwraps single items instead of writing an array.</param>
	[RequiresDynamicCode("Calls JsonSerializer.Serialize. Make sure the options object contains all relevant JsonTypeInfos before suppressing this warning.")]
	[RequiresUnreferencedCode("Calls JsonSerializer.Serialize. Make sure the options object contains all relevant JsonTypeInfos before suppressing this warning.")]
	public static void WriteRules(this Utf8JsonWriter writer, IEnumerable<Rule> rules, JsonSerializerOptions options, bool unwrapSingle = true)
	{
		var array = rules.ToArray();
		if (unwrapSingle && array.Length == 1)
		{
			writer.WriteRule(array[0], options);
			return;
		}

		writer.WriteStartArray();
		foreach (var rule in array)
		{
			writer.WriteRule(rule, options);
		}

		writer.WriteEndArray();
	}
}