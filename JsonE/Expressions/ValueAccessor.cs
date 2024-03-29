﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using Json.JsonE.Operators;

namespace Json.JsonE.Expressions;

internal class ValueAccessor
{
	private readonly IContextAccessorSegment[] _segments;
	private readonly string _asString;

	private ValueAccessor(IEnumerable<IContextAccessorSegment> segments, string asString)
	{
		_segments = segments.ToArray();
		_asString = asString;
	}

	internal static bool TryParse(ReadOnlySpan<char> source, ref int index, out ValueAccessor? accessor)
	{
		var i = index;
		if (!source.ConsumeWhitespace(ref i))
		{
			accessor = null;
			return false;
		}

		var segments = new List<IContextAccessorSegment>();

		while (i < source.Length)
		{
			if (!source.ConsumeWhitespace(ref i))
			{
				accessor = null;
				return false;
			}

			switch (source[i])
			{
				case '.':
					i++;
					if (!source.TryParseName(ref i, out var name))
						throw new SyntaxException(CommonErrors.WrongToken(source[i], "identifier"));

					segments.Add(new PropertySegment(name!, false));
					continue;
				case '[':
					i++;

					if (!source.ConsumeWhitespace(ref i))
					{
						accessor = null;
						return false;
					}

					if (!TryParseQuotedName(source, ref i, out var segment) &&
					    !TryParseSlice(source, ref i, out segment) &&
					    !TryParseIndex(source, ref i, out segment) &&
					    !TryParseExpression(source, ref i, out segment))
						throw new SyntaxException(CommonErrors.WrongToken(source[i]));

					segments.Add(segment!);

					if (!source.ConsumeWhitespace(ref i))
						throw new SyntaxException("Missing closing ]");
					if (source[i] != ']')
						throw new SyntaxException(CommonErrors.WrongToken(source[i], "]"));

					i++;

					continue;
			}

			break;
		}

		if (segments.Count == 0)
		{
			accessor = null;
			return false;
		}

		var asString = source[index..i].ToString();
		index = i;
		accessor = new ValueAccessor(segments, asString);
		return true;
	}

	private static bool TryParseQuotedName(ReadOnlySpan<char> source, ref int index, out IContextAccessorSegment? segment)
	{
		char quoteChar;
		var i = index;
		switch (source[index])
		{
			case '"':
				quoteChar = '"';
				i++;
				break;
			case '\'':
				quoteChar = '\'';
				i++;
				break;
			default:
				segment = null;
				return false;
		}

		var done = false;
		var sb = new StringBuilder();
		while (i < source.Length && !done)
		{
			if (source[i] == quoteChar)
			{
				done = true;
				i++;
			}
			else
			{
				if (!source.EnsureValidNameCharacter(i))
				{
					segment = null;
					return false;
				}
				sb.Append(source[i]);
				i++;
			}
		}

		if (!done)
		{
			segment = null;
			return false;
		}

		index = i;
		segment = new PropertySegment(sb.ToString(), true);
		return true;

	}

	private static bool TryParseIndex(ReadOnlySpan<char> source, ref int index, out IContextAccessorSegment? segment)
	{
		if (!source.TryGetInt(ref index, out var i))
		{
			segment = null;
			return false;
		}

		segment = new IndexSegment(i);
		return true;
	}

	private static bool TryParseSlice(ReadOnlySpan<char> source, ref int index, out IContextAccessorSegment? segment)
	{
		var i = index;

		ExpressionParser.TryParse(source, ref i, out var start);

		if (!source.ConsumeWhitespace(ref i))
		{
			segment = null;
			return false;
		}

		if (source[i] != ':')
		{
			segment = null;
			return false;
		}

		i++; // consume :

		ExpressionNode? step = null;
		ExpressionNode? end = null;
		if (source.ConsumeWhitespace(ref i))
		{
			ExpressionParser.TryParse(source, ref i, out end);

			if (source.ConsumeWhitespace(ref i))
			{
				if (source[i] == ':')
				{
					i++; // consume :

					if (source.ConsumeWhitespace(ref i))
					{
						ExpressionParser.TryParse(source, ref i, out step);
					}
				}
			}
		}

		index = i;
		segment = new SliceSegment(start, end, step);
		return true;
	}

	private static bool TryParseExpression(ReadOnlySpan<char> source, ref int i, out IContextAccessorSegment? segment)
	{
		if (!ExpressionParser.TryParse(source, ref i, out var expression))
		{
			segment = null;
			return false;
		}

		segment = new ExpressionSegment(expression!);
		return true;
	}

	internal JsonNode? Find(JsonNode? localContext, EvaluationContext fullContext)
	{
		var current = localContext;
		foreach (var segment in _segments)
		{
			if (!segment.TryFind(current, fullContext, out var value))
			{
				if (current is JsonObject)
					throw new InterpreterException($"object has no property \"{segment}\"");
				throw new InterpreterException($"unknown context value \"{segment}\"");
			}

			current = value;
		}

		return current;
	}

	public override string ToString() => _asString;
}