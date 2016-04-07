namespace Gu.State.Benchmarks
{
    using BenchmarkDotNet.Running;

    public class Program
    {
        static void Main()
        {
            var summary = BenchmarkRunner.Run<EqualByBenchmarks>();
        }
    }
}
