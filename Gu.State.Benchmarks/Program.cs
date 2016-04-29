namespace Gu.State.Benchmarks
{
    using BenchmarkDotNet.Running;

    public class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<EqualByBenchmarks>();
        }
    }
}
