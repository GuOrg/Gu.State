namespace Gu.State.Benchmarks
{
    using BenchmarkDotNet.Running;

    public class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<EqualByComplexTypeBenchmarks>();
            BenchmarkRunner.Run<CopyArray2DBenchmarks>();
            BenchmarkRunner.Run<CopyArray3DBenchmarks>();
            BenchmarkRunner.Run<CopyListBenchmarks>();
        }
    }
}
