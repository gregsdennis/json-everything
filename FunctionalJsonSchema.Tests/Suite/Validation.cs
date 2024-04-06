﻿using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Json.More;
#pragma warning disable NUnit2005

namespace FunctionalJsonSchema.Tests.Suite;

public class Validation
{
	private const string _testCasesPath = @"../../../../ref-repos/JSON-Schema-Test-Suite/tests";
	private const string _remoteSchemasPath = @"../../../../ref-repos/JSON-Schema-Test-Suite/remotes";

	private const bool _useExternal = false;
	private const bool _runDraftNext = true;
	private const string _externalTestCasesPath = @"../../../../../JSON-Schema-Test-Suite/tests";
	private const string _externalRemoteSchemasPath = @"../../../../../JSON-Schema-Test-Suite/remotes";

	private static readonly JsonSerializerOptions _testFileSerializationOptions = new()
	{
		WriteIndented = true,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		PropertyNameCaseInsensitive = true
	};

	public static IEnumerable<TestCaseData> TestCases()
	{
		return GetTests("draft6")
			.Concat(GetTests("draft7"))
			.Concat(GetTests("draft2019-09"))
			.Concat(GetTests("draft2020-12"))
			.Concat(_runDraftNext ? GetTests("draft-next") : Enumerable.Empty<TestCaseData>());
	}

	private static IEnumerable<TestCaseData> GetTests(string draftFolder)
	{
		// ReSharper disable once HeuristicUnreachableCode
		var testCasesPath = _useExternal ? _externalTestCasesPath : _testCasesPath;

		var testsPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, testCasesPath, $"{draftFolder}/")
			.AdjustForPlatform();
		if (!Directory.Exists(testsPath)) return Enumerable.Empty<TestCaseData>();

		var fileNames = Directory.GetFiles(testsPath, "*.json", SearchOption.AllDirectories);

		var allTests = new List<TestCaseData>();
		foreach (var fileName in fileNames)
		{
			var shortFileName = Path.GetFileNameWithoutExtension(fileName);

			// adjust for format
			var options = new EvaluationOptions();
			options.DefaultMetaSchema = draftFolder switch
			{
				"draft6" => MetaSchemas.Draft6Id,
				"draft7" => MetaSchemas.Draft7Id,
				"draft2019-09" => MetaSchemas.Draft201909Id,
				"draft2020-12" => MetaSchemas.Draft202012Id,
				"draft-next" => MetaSchemas.DraftNextId,
				_ => options.DefaultMetaSchema
			};
			options.RequireFormatValidation = fileName.Contains("format/".AdjustForPlatform()) &&
			                                  // uri-template will throw an exception as it's explicitly unsupported
			                                  shortFileName != "uri-template";

			var contents = File.ReadAllText(fileName);
			var collections = JsonSerializer.Deserialize<List<TestCollection>>(contents, _testFileSerializationOptions);

			foreach (var collection in collections!)
			{
				collection.IsOptional = fileName.Contains("optional");
				foreach (var test in collection.Tests)
				{
					var optional = collection.IsOptional ? "(optional) / " : null;
					var name = $"{draftFolder} / {shortFileName} / {optional}{collection.Description} / {test.Description}";
					allTests.Add(new TestCaseData(collection, test, shortFileName, options) { TestName = name });
				}
			}
		}

		return allTests;
	}

	[OneTimeSetUp]
	public void LoadRemoteSchemas()
	{
		// ReSharper disable once HeuristicUnreachableCode
		var remotesPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, _useExternal ? _externalRemoteSchemasPath : _remoteSchemasPath)
			.AdjustForPlatform();
		if (!Directory.Exists(remotesPath)) throw new Exception("Cannot find remotes folder");

		var fileNames = Directory.GetFiles(remotesPath, "*.json", SearchOption.AllDirectories);

		foreach (var fileName in fileNames)
		{
			var schema = (JsonObject)JsonNode.Parse(File.ReadAllText(fileName))!;
			var uri = new Uri(fileName.Replace(remotesPath, "http://localhost:1234").Replace('\\', '/'));
			EvaluationOptions.Default.SchemaRegistry.Register(uri, schema);
		}
	}

	[TestCaseSource(nameof(TestCases))]
	public void Test(TestCollection collection, TestCase test, string fileName, EvaluationOptions options)
	{
		Console.WriteLine();
		Console.WriteLine();
		Console.WriteLine(fileName);
		Console.WriteLine(collection.Description);
		Console.WriteLine(test.Description);
		Console.WriteLine(test.Valid ? "valid" : "invalid");
		Console.WriteLine();
		Console.WriteLine(JsonSerializer.Serialize(collection.Schema, _testFileSerializationOptions));
		Console.WriteLine();
		Console.WriteLine(test.Data.AsJsonString());
		Console.WriteLine();

		if (!InstanceIsDeserializable(test.Data))
			Assert.Inconclusive("Instance not deserializable");

		EvaluationResults result;
		try
		{
			result = JsonSchema.Evaluate(collection.Schema!, test.Data, options);
		}
		catch (RegexParseException e)
		{
			Console.WriteLine(e);
			if (collection.IsOptional)
				Assert.Inconclusive("Test optional");

			throw;
		}
		//result.ToBasic();
		Console.WriteLine(JsonSerializer.Serialize(result, _testFileSerializationOptions));

		if (collection.IsOptional && result.Valid != test.Valid)
			Assert.Inconclusive("Test optional");

		Assert.AreEqual(test.Valid, result.Valid);
	}

	private static bool InstanceIsDeserializable(in JsonNode? testData)
	{
		try
		{
			var value = (testData as JsonValue)?.GetValue<object>();
			if (value is null) return true;
			if (value is string) return false;
			if (value is JsonElement { ValueKind: JsonValueKind.Number } element)
				return element.TryGetDecimal(out _);
			if (value.GetType().IsNumber())
			{
				// some tests involve numbers larger than c# can handle.  fortunately, they're optional.
				return true;
			}

			return true;
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return false;
		}
	}

	[Test]
	public void EnsureTestSuiteConfiguredForServerBuild()
	{
		Assert.IsFalse(_useExternal);
		//Assert.IsFalse(_runDraftNext);
	}
}