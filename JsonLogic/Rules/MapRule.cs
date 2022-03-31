﻿using System;
using System.Linq;
using System.Text.Json;
using Json.More;

namespace Json.Logic.Rules;

[Operator("map")]
internal class MapRule : Rule
{
	private readonly Rule _input;
	private readonly Rule _rule;

	public MapRule(Rule input, Rule rule)
	{
		_input = input;
		_rule = rule;
	}

	public override JsonElement Apply(JsonElement data)
	{
		var input = _input.Apply(data);

		if (input.ValueKind != JsonValueKind.Array)
			return Array.Empty<JsonElement>().AsJsonElement();

		return input.EnumerateArray().Select(i => _rule.Apply(i)).AsJsonElement();
	}
}