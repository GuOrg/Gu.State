namespace Gu.State.Benchmarks
{
    using System.Collections.Generic;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    [MemoryDiagnoser]
    public class CopyList
    {
        private readonly List<int> source;
        private readonly List<int> target;

        public CopyList()
        {
            this.source = Enumerable.Range(0, 1000).ToList();
            this.target = Enumerable.Range(0, 1000).ToList();
        }

        [Benchmark(Baseline = true)]
        public void ForLoop()
        {
            for (int i = 0; i < this.source.Count; i++)
            {
                this.target[i] = this.source[i];
            }
        }

        [Benchmark]
        public void CopyPropertyValues()
        {
            Copy.PropertyValues(this.source, this.target);
        }

        [Benchmark]
        public void CopyFieldValues()
        {
            Copy.FieldValues(this.source, this.target);
        }
    }
}