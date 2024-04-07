using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Json.Schema.Benchmark.Functional;
using Json.Schema.Benchmark.Suite;

namespace Json.Schema.Benchmark;

class Program
{
	static void Main(string[] args)
	{
#if DEBUG
		//IConfig config = new DebugBuildConfig();
		//config.WithOptions(ConfigOptions.DisableOptimizationsValidator);
		//var summary = BenchmarkRunner.Run<BothRunner>(config);

		var runner = new ExperimentalSuiteRunner();
		runner.BenchmarkSetup();
		runner.RunSuite(1);
#else
		var summary = BenchmarkRunner.Run<BothRunner>();
#endif
	}
}