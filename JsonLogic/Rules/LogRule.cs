﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Json.More;

namespace Json.Logic.Rules;

/// <summary>
/// Handles the `log` operation.
/// </summary>
[Operator("log")]
[JsonConverter(typeof(LogRuleJsonConverter))]
public class LogRule : Rule
{
	internal Rule Log { get; }

	internal LogRule(Rule log)
	{
		Log = log;
	}

	/// <summary>
	/// Applies the rule to the input data.
	/// </summary>
	/// <param name="data">The input data.</param>
	/// <param name="contextData">
	///     Optional secondary data.  Used by a few operators to pass a secondary
	///     data context to inner operators.
	/// </param>
	/// <returns>The result of the rule.</returns>
	public override JsonNode? Apply(JsonNode? data, JsonNode? contextData = null)
	{
		var log = Log.Apply(data, contextData);

		Console.WriteLine(log);

		return log;
	}
}

internal class LogRuleJsonConverter : AotCompatibleJsonConverter<LogRule>
{
	public override LogRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var parameters = reader.TokenType == JsonTokenType.StartArray
			? options.Read(ref reader, LogicSerializerContext.Default.RuleArray)
			: new[] { options.Read(ref reader, LogicSerializerContext.Default.Rule)! };

		return new LogRule(parameters!.Length == 0
			? new LiteralRule("")
			: parameters[0]);
	}

	[UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "We guarantee that the SerializerOptions covers all the types we need for AOT scenarios.")]
	[UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "We guarantee that the SerializerOptions covers all the types we need for AOT scenarios.")]
	public override void Write(Utf8JsonWriter writer, LogRule value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("log");
		writer.WriteRule(value.Log, options);
		writer.WriteEndObject();
	}
}
