using Json.Logic.Rules;
using NUnit.Framework;

namespace Json.Logic.Tests;

public class MoreThanTests
{
	[Test]
	public void MoreThanTwoNumbersReturnsTrue()
	{
		var rule = new MoreThanRule(2, 1);

		JsonAssert.IsTrue(rule.Apply());
	}

	[Test]
	public void EqualTwoNumbersReturnsFalse()
	{
		var rule = new MoreThanRule(1, 1);

		JsonAssert.IsFalse(rule.Apply());
	}

	[Test]
	public void MoreThanTwoNumbersReturnsFalse()
	{
		var rule = new MoreThanRule(1, 2);

		JsonAssert.IsFalse(rule.Apply());
	}

	[Test]
	public void MoreThanBooleanThrowsError()
	{
		var rule = new MoreThanRule(false, 2);

		JsonAssert.IsFalse(rule.Apply());
	}

	[Test]
	public void MoreThanNullCastsNullToZero()
	{
		var rule = new MoreThanRule(LiteralRule.Null, 2);

		JsonAssert.IsFalse(rule.Apply());
	}
}