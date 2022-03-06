namespace Gu.State.Benchmarks
{
    using BenchmarkDotNet.Attributes;

    [MemoryDiagnoser]
    public class CopyArray2D
    {
        private readonly int[,] source;
        private readonly int[,] target;

        public CopyArray2D()
        {
            this.source = new int[100, 100];
            this.target = new int[100, 100];
        }

        [Benchmark(Baseline = true)]
        public void ForLoops()
        {
            for (int i = this.source.GetLowerBound(0); i < this.source.GetUpperBound(0); i++)
            {
                for (int j = this.source.GetLowerBound(1); j < this.source.GetUpperBound(1); j++)
                {
                    this.target[i, j] = this.source[i, j];
                }
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
