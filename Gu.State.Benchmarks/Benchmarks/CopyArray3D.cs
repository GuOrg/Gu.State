namespace Gu.State.Benchmarks
{
    using BenchmarkDotNet.Attributes;

    [MemoryDiagnoser]
    public class CopyArray3D
    {
        private readonly int[,,] source;
        private readonly int[,,] target;

        public CopyArray3D()
        {
            this.source = new int[10, 10, 10];
            this.target = new int[10, 10, 10];
        }

        [Benchmark(Baseline = true)]
        public void Forloops()
        {
            for (int i = this.source.GetLowerBound(0); i < this.source.GetUpperBound(0); i++)
            {
                for (int j = this.source.GetLowerBound(1); j < this.source.GetUpperBound(1); j++)
                {
                    for (int k = this.source.GetLowerBound(2); k < this.source.GetUpperBound(2); k++)
                    {
                        this.target[i, j, k] = this.source[i, j, k];
                    }
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
