﻿using System.Text.Json.Nodes;
using Json.JsonE.Operators;
using Json.More;

namespace Json.JsonE.Expressions.Functions;

internal class StrFunction : FunctionDefinition
{
	private const string _name = "str";

	internal override JsonNode? Invoke(JsonNode?[] arguments, EvaluationContext context)
	{
		if (arguments[0] is null) return "null";

		if (arguments[0] is not JsonValue val)
			throw new BuiltInException(CommonErrors.IncorrectArgType(_name));

		if (val.TryGetValue(out string? str)) return str;
		if (val.TryGetValue(out bool b)) return b ? "true" : "false";

		var num = val.GetNumber();
		if (!num.HasValue)
			throw new BuiltInException(CommonErrors.IncorrectArgType(_name));

		return num.ToString();
	}
}