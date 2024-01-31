using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NUnit.Framework;

namespace Json.More.Tests;

public class EnumStringConverterTests
{
	private class ConversionTest
	{
		[JsonConverter(typeof(EnumStringConverter<DayOfWeek>))]
		public DayOfWeek Day { get; set; }
	}

	[Test]
	public void DayOfWeekAsPropertyIsConverted()
	{
		var expected = "{\"Day\":\"Wednesday\"}";
		var actual = JsonSerializer.Serialize(new ConversionTest { Day = DayOfWeek.Wednesday });

		Assert.AreEqual(expected, actual);

		var deserialized = JsonSerializer.Deserialize<ConversionTest>(actual)!;

		Assert.AreEqual(DayOfWeek.Wednesday, deserialized.Day);
	}

	[JsonConverter(typeof(EnumStringConverter<CustomEnum>))]
	public enum CustomEnum
	{
		Zero,
		[System.ComponentModel.Description("one")]
		One,
		Two
	}

	[TestCase(CustomEnum.Zero, "Zero")]
	[TestCase(CustomEnum.One, "one")]
	[TestCase(CustomEnum.Two, "Two")]
	public void CustomEnumIsConverted(CustomEnum value, string serializedValue)
	{
		var expected = $"\"{serializedValue}\"";
		var actual = JsonSerializer.Serialize(value);

		Assert.AreEqual(expected, actual);

		var deserialized = JsonSerializer.Deserialize<CustomEnum>(actual);

		Assert.AreEqual(value, deserialized);
	}

	[JsonConverter(typeof(EnumStringConverter<CustomFlagsEnum>))]
	[Flags]
	private enum CustomFlagsEnum
	{
		Zero,
		One = 1,
		Two = 2
	}

	private class FlagsEnumContainer
	{
		public CustomFlagsEnum Value { get; set; }
	}

	[Test]
	public void CustomFlagsEnumIsConverted()
	{
		var value = new FlagsEnumContainer { Value = CustomFlagsEnum.One | CustomFlagsEnum.Two };

		var expected = "{\"Value\":[\"One\",\"Two\"]}";
		var actual = JsonSerializer.Serialize(value);

		Assert.AreEqual(expected, actual);

		var deserialized = JsonSerializer.Deserialize<FlagsEnumContainer>(actual)!;

		Assert.AreEqual(CustomFlagsEnum.One | CustomFlagsEnum.Two, deserialized.Value);
	}
}