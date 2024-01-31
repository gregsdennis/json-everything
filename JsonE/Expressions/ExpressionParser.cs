﻿using System;
using System.Collections.Generic;
using Json.JsonE.Expressions.Operators;
using static Json.JsonE.Operators.CommonErrors;

namespace Json.JsonE.Expressions;

internal static class ExpressionParser
{
	private static readonly IOperandExpressionParser[] _operandParsers =
	{
		new ObjectExpressionParser(),
		new ArrayExpressionParser(),
		new UnaryExpressionParser(),
		new ContextAccessorExpressionParser(),
		new PrimitiveExpressionParser(),
	};

	public static ExpressionNode Parse(ReadOnlySpan<char> source)
	{
		int index = 0;
		if (!TryParse(source, ref index, out var expression) || index != source.Length)
			throw new TemplateException("Expression is not valid");

		return expression!;
	}

	public static bool TryParse(ReadOnlySpan<char> source, ref int index, out ExpressionNode? expression, bool skipFunctions = false)
	{
		int i = index;
		var nestLevel = 0;

		int Precedence(IBinaryOperator op) => nestLevel * 10 + op.Precedence;

		if (!source.ConsumeWhitespace(ref index))
		{
			expression = null;
			return false;
		}

		var isGroup = false;
		while (i < source.Length && source[i] == '(')
		{
			nestLevel++;
			i++;
			isGroup = true;
		}

		if (i == source.Length)
			throw new TemplateException(EndOfInput(i));

		// first get an operand
		ExpressionNode? left = null;
		foreach (var parser in _operandParsers)
		{
			if (parser.TryParse(source, ref i, out left)) break;
		}

		if (left == null)
		{
			expression = null;
			return false;
		}

		bool stillBuildingOperand;
		ValueAccessor? valueAccessor;
		List<ExpressionNode>? functionArguments;
		do
		{
			stillBuildingOperand = false;
			if (ValueAccessor.TryParse(source, ref i, out valueAccessor))
			{
				left = new ValueAccessorExpressionNode(left, valueAccessor!);
				stillBuildingOperand = true;
			}

			if (FunctionArgumentParser.TryParse(source, ref i, out functionArguments))
			{
				left = new FunctionExpressionNode(left, functionArguments!);
				stillBuildingOperand = true;
			}
		} while (stillBuildingOperand);

		while (i < source.Length)
		{
			// handle )
			if (!source.ConsumeWhitespace(ref index))
			{
				expression = null;
				return false;
			}

			if (source[i] == ')' && nestLevel > 0)
			{
				while (i < source.Length && source[i] == ')' && nestLevel > 0)
				{
					nestLevel--;
					i++;
				}

				if (nestLevel == 0) continue;
			}

			var nextNest = nestLevel;
			// parse operator
			if (!OperatorRepository.TryGetBinary(source, ref i, out var op) || op is not IBinaryOperator binOp)
				break; // if we don't get a binary op, then we're done

			// handle (
			if (!source.ConsumeWhitespace(ref i))
			{
				expression = null;
				return false;
			}

			if (source[i] == '(')
			{
				nextNest++;
				i++;
			}

			// parse right
			ExpressionNode? right = null;
			foreach (var parser in _operandParsers)
			{
				if (parser.TryParse(source, ref i, out right)) break;
			}

			if (right == null)
			{
				// if we don't get a value, then the syntax is wrong
				expression = null;
				return false;
			}

			do
			{
				stillBuildingOperand = false;
				if (ValueAccessor.TryParse(source, ref i, out valueAccessor))
				{
					right = new ValueAccessorExpressionNode(right, valueAccessor!);
					stillBuildingOperand = true;
				}

				if (FunctionArgumentParser.TryParse(source, ref i, out functionArguments))
				{
					right = new FunctionExpressionNode(right, functionArguments!);
					stillBuildingOperand = true;
				}
			} while (stillBuildingOperand);

			if (left is BinaryExpressionNode bin)
			{
				if (bin.Precedence < Precedence(binOp) || (bin.Operator is ExponentOperator && binOp is ExponentOperator))
				{
					while (bin.Right is BinaryExpressionNode bRight && (bin.Precedence < Precedence(binOp) || (bin.Operator is ExponentOperator && binOp is ExponentOperator)))
					{
						bin = bRight;
					}

					bin.Right = new BinaryExpressionNode(binOp, bin.Right, right, nestLevel);
				}
				else
					left = new BinaryExpressionNode(binOp, left, right, nestLevel);
			}
			else
				left = new BinaryExpressionNode(binOp, left, right, nestLevel);

			nestLevel = nextNest;
		}

		if (nestLevel > 0)
		{
			expression = null;
			return false;
		}

		if (isGroup)
		{
			if (ValueAccessor.TryParse(source, ref i, out valueAccessor))
				left = new ValueAccessorExpressionNode(left, valueAccessor!);

			if (FunctionArgumentParser.TryParse(source, ref i, out functionArguments))
				left = new FunctionExpressionNode(left, functionArguments!);
		}

		index = i;
		expression = left;
		return true;
	}

}