﻿using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Json.More;

namespace Json.Logic.Tests.Suite;

[JsonConverter(typeof(TestSuiteConverter))]
public class TestSuite
{
#pragma warning disable CS8618
	public Test[] Tests { get; set; }
#pragma warning restore CS8618
}

public class TestSuiteConverter : JsonConverter<TestSuite?>
{
	public override TestSuite? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException("Test suite must be an array of tests.");

		var tests = options.Read(ref reader, TestSerializerContext.Default.TestArray)!
			.Where(t => t != null!)
			.ToArray();

		return new TestSuite { Tests = tests };
	}

	public override void Write(Utf8JsonWriter writer, TestSuite? value, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}
}
