using System.Collections.Generic;
using System.Text.Json.Nodes;

using ExperimentsOptions = Json.Schema.Experiments.EvaluationOptions;

#pragma warning disable CS8618

namespace Json.Schema.Benchmark.Suite;

public class TestCollection
{
	public string Description { get; set; }
	public JsonSchema Schema { get; set; }
	// ReSharper disable once CollectionNeverUpdated.Global
	public List<TestCase> Tests { get; set; }
	public bool IsOptional { get; set; }
	public EvaluationOptions Options { get; set; }
}

public class ExperimentalTestCollection
{
	public string Description { get; set; }
	public JsonNode Schema { get; set; }
	// ReSharper disable once CollectionNeverUpdated.Global
	public List<TestCase> Tests { get; set; }
	public bool IsOptional { get; set; }
	public ExperimentsOptions Options { get; set; }
}