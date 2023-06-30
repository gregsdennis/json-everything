﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Json.More;

namespace Json.Schema;

/// <summary>
/// Handles `else`.
/// </summary>
[SchemaKeyword(Name)]
[SchemaPriority(10)]
[SchemaSpecVersion(SpecVersion.Draft7)]
[SchemaSpecVersion(SpecVersion.Draft201909)]
[SchemaSpecVersion(SpecVersion.Draft202012)]
[SchemaSpecVersion(SpecVersion.DraftNext)]
[Vocabulary(Vocabularies.Applicator201909Id)]
[Vocabulary(Vocabularies.Applicator202012Id)]
[Vocabulary(Vocabularies.ApplicatorNextId)]
[DependsOnAnnotationsFrom(typeof(IfKeyword))]
[JsonConverter(typeof(ElseKeywordJsonConverter))]
public class ElseKeyword : IJsonSchemaKeyword, ISchemaContainer, IEquatable<ElseKeyword>
{
	/// <summary>
	/// The JSON name of the keyword.
	/// </summary>
	public const string Name = "else";

	/// <summary>
	/// The schema to match.
	/// </summary>
	public JsonSchema Schema { get; }

	/// <summary>
	/// Creates a new <see cref="ElseKeyword"/>.
	/// </summary>
	/// <param name="value">The schema to match.</param>
	public ElseKeyword(JsonSchema value)
	{
		Schema = value ?? throw new ArgumentNullException(nameof(value));
	}

	/// <summary>
	/// Performs evaluation for the keyword.
	/// </summary>
	/// <param name="context">Contextual details for the evaluation process.</param>
	/// <param name="token">A cancellation token to know if other branches of the schema have completed in an optimized evaluation.</param>
	public async Task Evaluate(EvaluationContext context, CancellationToken token)
	{
		context.EnterKeyword(Name);
		if (!context.LocalResult.TryGetAnnotation(IfKeyword.Name, out var annotation))
		{
			context.NotApplicable(() => $"No annotation found for {IfKeyword.Name}.");
			return;
		}

		context.Log(() => $"Annotation for {IfKeyword.Name} is {annotation.AsJsonString()}.");
		var ifResult = annotation!.GetValue<bool>();
		if (ifResult)
		{
			context.NotApplicable(() => $"{Name} does not apply.");
			return;
		}

		context.Push(context.EvaluationPath.Combine(Name), Schema);
		await context.Evaluate(token);
		var valid = context.LocalResult.IsValid;
		context.Pop();
		if (!valid) 
			context.LocalResult.Fail();
		context.ExitKeyword(Name, context.LocalResult.IsValid);
	}

	/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
	/// <param name="other">An object to compare with this object.</param>
	/// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
	public bool Equals(ElseKeyword? other)
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
		return Equals(obj as ElseKeyword);
	}

	/// <summary>Serves as the default hash function.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override int GetHashCode()
	{
		return Schema.GetHashCode();
	}
}

internal class ElseKeywordJsonConverter : JsonConverter<ElseKeyword>
{
	public override ElseKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var schema = JsonSerializer.Deserialize<JsonSchema>(ref reader, options)!;

		return new ElseKeyword(schema);
	}
	public override void Write(Utf8JsonWriter writer, ElseKeyword value, JsonSerializerOptions options)
	{
		writer.WritePropertyName(ElseKeyword.Name);
		JsonSerializer.Serialize(writer, value.Schema, options);
	}
}