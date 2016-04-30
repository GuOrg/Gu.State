namespace Gu.State.Benchmarks
{
    using System.Collections.Generic;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    public class CopyListBenchmarks
    {
        private readonly List<int> source;
        private readonly List<int> target;

        public CopyListBenchmarks()
        {
            this.source = Enumerable.Range(0, 1000).ToList();
            this.target = Enumerable.Range(0, 1000).ToList();
        }

        [Benchmark(Baseline = true)]
        public void Baseline()
        {
            for (int i = 0; i < this.source.Count; i++)
            {
                this.target[i] = this.source[i];
            }
        }

        [Benchmark]
        public void CopyFieldValues()
        {
            Copy.FieldValues(this.source, this.target);
        }
    }
}