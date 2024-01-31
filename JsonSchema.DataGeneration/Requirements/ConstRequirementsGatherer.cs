﻿using System.Linq;

namespace Json.Schema.DataGeneration.Requirements;

internal class ConstRequirementsGatherer : IRequirementsGatherer
{
	public void AddRequirements(RequirementsContext context, JsonSchema schema)
	{
		var constKeyword = schema.Keywords?.OfType<ConstKeyword>().FirstOrDefault();
		if (constKeyword != null)
		{
			if (context.ConstIsSet)
				context.HasConflict = true;
			else
			{
				context.Const = constKeyword.Value;
				context.ConstIsSet = true;
			}
		}
	}
}