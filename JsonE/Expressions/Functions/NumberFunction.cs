﻿using System.Text.Json.Nodes;
using Json.JsonE.Operators;

namespace Json.JsonE.Expressions.Functions;

internal class NumberFunction : FunctionDefinition
{
	private const string _name = "number";

	internal override JsonNode? Invoke(JsonNode?[] arguments, EvaluationContext context)
	{
		if (arguments[0] is not JsonValue val || !val.TryGetValue(out string? str))
			throw new InterpreterException(CommonErrors.IncorrectArgType(_name));

		return decimal.TryParse(str, out var d)
			? d
			: throw new TemplateException("not a number");
	}
}