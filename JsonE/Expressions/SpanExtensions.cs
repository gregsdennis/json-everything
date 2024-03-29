﻿using System;
using System.Text.Json.Nodes;

namespace Json.JsonE.Expressions;

internal static class SpanExtensions
{
	public static bool ConsumeWhitespace(this ReadOnlySpan<char> span, ref int i)
	{
		while (i < span.Length && char.IsWhiteSpace(span[i]))
		{
			i++;
		}
		return i != span.Length;
	}

	public static bool EnsureValidNameCharacter(this ReadOnlySpan<char> span, int i)
	{
		return span[i] >= 0x20;
		//throw new PathParseException(i, "Characters in the range U+0000..U+001F are disallowed");
	}

	public static bool TryGetInt(this ReadOnlySpan<char> span, ref int index, out int value)
	{
		var negative = false;
		var i = index;
		if (span[i] == '-')
		{
			negative = true;
			i++;
		}

		// Now move past digits
		var foundNumber = false;
		var zeroStart = false;
		long parsedValue = 0;
		var overflowed = false;
		value = 0;
		while (i < span.Length && char.IsDigit(span[i]))
		{
			if (zeroStart) return false;

			if (!foundNumber && span[i] == '0')
				zeroStart = true;

			foundNumber = true;
			if (!overflowed)
			{
				parsedValue = parsedValue * 10 + span[i] - '0';
				overflowed = parsedValue is <= -2L << 53 or >= 2L << 53;
			}

			i++;
		}

		if (overflowed) return false;

		if (negative) parsedValue = -parsedValue;

		index = i;
		value = (int)Math.Min(int.MaxValue, Math.Max(int.MinValue, parsedValue));
		return foundNumber;
	}

	public static bool TryParsePrimitive(this ReadOnlySpan<char> span, ref int i, out JsonNode? node)
	{
		if (!span.ConsumeWhitespace(ref i))
		{
			node = null;
			return false;
		}

		try
		{
			int end = i;
			switch (span[i])
			{
				case 'f':
					end += 5;
					break;
				case 't':
				case 'n':
					end += 4;
					break;
				case '.':
				case '-':
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					end = i;
					var allowDash = true;
					while (end < span.Length && (span[end].In('0'..('9' + 1)) ||
					                             span[end].In('e', '.', '-', '+')))
					{
						if (!allowDash && span[end] is '-' or '+') break;
						allowDash = span[end] == 'e';
						end++;
					}
					node = decimal.Parse(span[i..end].ToString());
					i = end;
					return true;
				case '\'':
				case '"':
					end = i + 1;
					var endChar = span[i];
					while (end < span.Length && span[end] != endChar)
					{
						if (span[end] == '\\')
						{
							end++;
							if (end >= span.Length) break;
						}
						end++;
					}

					node = span[(i + 1)..end].ToString();
					i = end+1;
					return true;
				default:
					node = default;
					return false;
			}

			var block = span[i..end];
			node = JsonNode.Parse(block.ToString());
			i = end;
			return true;
		}
		catch
		{
			node = default;
			return false;
		}
	}

	public static bool TryParseName(this ReadOnlySpan<char> source, ref int index, out string? name)
	{
		var i = index;

		if (!source.ConsumeWhitespace(ref i) || i == source.Length)
		{
			name = null;
			return false;
		}

		if (i < source.Length && source[i].IsValidForPropertyNameStart())
		{
			i++;
			while (i < source.Length && source[i].IsValidForPropertyName())
			{
				i++;
			}
		}

		if (index == i)
		{
			name = null;
			return false;
		}

		name = source[index..i].ToString();
		index = i;
		return true;
	}

	private static bool IsValidForPropertyName(this char ch)
	{
		return ch.In('a'..('z' + 1)) ||
		       ch.In('A'..('Z' + 1)) ||
		       ch.In('0'..('9' + 1)) ||
		       ch.In('_') ||
		       ch.In(0x80..0x10FFFF);
	}

	private static bool IsValidForPropertyNameStart(this char ch)
	{
		return ch.In('a'..('z' + 1)) ||
		       ch.In('A'..('Z' + 1)) ||
		       ch.In('_') ||
		       ch.In(0x80..0x10FFFF);
	}
}