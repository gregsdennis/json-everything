﻿using System;
using System.ComponentModel;
using System.Globalization;

namespace Json.Pointer;

internal sealed class JsonPointerTypeConverter : TypeConverter
{
	private static StandardValuesCollection? _standardValues;

	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
	{
		return sourceType == typeof(string) || sourceType == typeof(JsonPointer) || base.CanConvertFrom(context, sourceType);
	}

	public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
	{
		return destinationType == typeof(string) || destinationType == typeof(JsonPointer) || base.CanConvertTo(context, destinationType);
	}

	public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
	{
		if (value is string s)
			return JsonPointer.Parse(s);
			
		if (value is JsonPointer pointer)
			return JsonPointer.Create(pointer.Segments);

		throw GetConvertFromException(value);
	}

	public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
	{
		if (value is JsonPointer pointer)
		{
			if (destinationType == typeof(string))
				return pointer.ToString();
				
			if (destinationType == typeof(JsonPointer))
				return JsonPointer.Create(pointer.Segments);
		}

		throw GetConvertToException(value, destinationType);
	}

	public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

	public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => false;

	public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
	{
		return _standardValues ??= new StandardValuesCollection(new[] { JsonPointer.Empty });
	}

	public override bool IsValid(ITypeDescriptorContext? context, object? value)
	{
		if (value is string s)
		{
			return JsonPointer.TryParse(s, out _);
		}
		return value is JsonPointer;
	}
}