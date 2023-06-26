﻿using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Json.More;

namespace Json.Schema;

/// <summary>
/// Handles `additionalItems`.
/// </summary>
[SchemaPriority(10)]
[SchemaKeyword(Name)]
[SchemaSpecVersion(SpecVersion.Draft6)]
[SchemaSpecVersion(SpecVersion.Draft7)]
[SchemaSpecVersion(SpecVersion.Draft201909)]
[Vocabulary(Vocabularies.Applicator201909Id)]
[DependsOnAnnotationsFrom(typeof(ItemsKeyword))]
[JsonConverter(typeof(AdditionalItemsKeywordJsonConverter))]
public class AdditionalItemsKeyword : IJsonSchemaKeyword, ISchemaContainer, IEquatable<AdditionalItemsKeyword>
{
	/// <summary>
	/// The JSON name of the keyword.
	/// </summary>
	public const string Name = "additionalItems";

	/// <summary>
	/// The schema by which to evaluate additional items.
	/// </summary>
	public JsonSchema Schema { get; }

	/// <summary>
	/// Creates a new <see cref="AdditionalItemsKeyword"/>.
	/// </summary>
	/// <param name="value">The keyword's schema.</param>
	public AdditionalItemsKeyword(JsonSchema value)
	{
		Schema = value ?? throw new ArgumentNullException(nameof(value));
	}

	/// <summary>
	/// Performs evaluation for the keyword.
	/// </summary>
	/// <param name="context">Contextual details for the evaluation process.</param>
	/// <param name="token">The cancellation token used by the caller.</param>
	public async Task Evaluate(EvaluationContext context, CancellationToken token)
	{
		context.EnterKeyword(Name);
		var schemaValueType = context.LocalInstance.GetSchemaValueType();
		if (schemaValueType != SchemaValueType.Array)
		{
			context.WrongValueKind(schemaValueType);
			return;
		}

		context.Options.LogIndentLevel++;
		var overallResult = true;
		if (!context.LocalResult.TryGetAnnotation(ItemsKeyword.Name, out var annotation))
		{
			context.NotApplicable(() => $"No annotations from {ItemsKeyword.Name}.");
			return;
		}
		context.Log(() => $"Annotation from {ItemsKeyword.Name}: {annotation}.");
		if (annotation!.GetValue<object>() is bool)
		{
			context.ExitKeyword(Name, context.LocalResult.IsValid);
			return;
		}

		var startIndex = (int)annotation.AsValue().GetInteger()!;
		var array = (JsonArray)context.LocalInstance!;

		var tokenSource = new CancellationTokenSource();
		token.Register(tokenSource.Cancel);

		var tasks = Enumerable.Range(startIndex, array.Count - startIndex)
			.Select(i => Task.Run(async () =>
			{
				if (tokenSource.Token.IsCancellationRequested) return ((int?)null, (bool?)null);

				context.Log(() => $"Evaluating item at index {i}.");
				var item = array[i];
				var branch = context.ParallelBranch(context.InstanceLocation.Combine(i),
					item ?? JsonNull.SignalNode,
					context.EvaluationPath.Combine(Name),
					Schema);
				await branch.Evaluate(tokenSource.Token);
				context.Log(() => $"Item at index {i} {branch.LocalResult.IsValid.GetValidityString()}.");

				return (i, branch.LocalResult.IsValid);
			}, tokenSource.Token)).ToArray();

		if (tasks.Any())
		{
			if (context.ApplyOptimizations)
			{
				var failedValidation = await tasks.WhenAny(x => !x.Item2 ?? false, tokenSource.Token);
				tokenSource.Cancel();

				overallResult = failedValidation == null;
			}
			else
			{
				await Task.WhenAll(tasks);
				overallResult = tasks.All(x => x.Result.Item2 ?? true);
			}
		}

		context.Options.LogIndentLevel--;
		context.LocalResult.SetAnnotation(Name, true);

		if (!overallResult)
			context.LocalResult.Fail();
		context.ExitKeyword(Name, context.LocalResult.IsValid);
	}

	/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
	/// <param name="other">An object to compare with this object.</param>
	/// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
	public bool Equals(AdditionalItemsKeyword? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Equals(Schema, other.Schema);
	}

	/// <summary>Determines whether the specified object is equal to the current object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
	public override bool Equals(object obj)
	{
		return Equals(obj as AdditionalItemsKeyword);
	}

	/// <summary>Serves as the default hash function.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return Schema.GetHashCode();
	}
}

internal class AdditionalItemsKeywordJsonConverter : JsonConverter<AdditionalItemsKeyword>
{
	public override AdditionalItemsKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var schema = JsonSerializer.Deserialize<JsonSchema>(ref reader, options)!;

		return new AdditionalItemsKeyword(schema);
	}
	public override void Write(Utf8JsonWriter writer, AdditionalItemsKeyword value, JsonSerializerOptions options)
	{
		writer.WritePropertyName(AdditionalItemsKeyword.Name);
		JsonSerializer.Serialize(writer, value.Schema, options);
	}
}