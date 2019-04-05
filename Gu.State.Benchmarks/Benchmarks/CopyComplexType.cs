namespace Gu.State.Benchmarks
{
    using BenchmarkDotNet.Attributes;

    public class CopyComplexType
    {
        private readonly ComplexType source = new ComplexType();
        private readonly ComplexType target = new ComplexType();

        [Benchmark(Baseline = true)]
        public ComplexType ManualImplementation()
        {
            this.target.Value = this.source.Value;
            this.target.Name = this.source.Name;
            return this.target;
        }

        [Benchmark]
        public ComplexType CopyPropertyValues()
        {
            Copy.PropertyValues(this.source, this.target);
            return this.target;
        }

        [Benchmark]
        public ComplexType CopyFieldValues()
        {
            Copy.FieldValues(this.source, this.target);
            return this.target;
        }
    }
}