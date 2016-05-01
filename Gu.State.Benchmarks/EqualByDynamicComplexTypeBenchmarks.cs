namespace Gu.State.Benchmarks
{
    using System;
    using System.Linq.Expressions;

    using BenchmarkDotNet.Attributes;

    public class EqualByDynamicComplexTypeBenchmarks
    {
        private readonly ComplexType x;
        private readonly ComplexType y;

        private Func<ComplexType, ComplexType, bool> compare;

        public EqualByDynamicComplexTypeBenchmarks()
        {
            this.x = new ComplexType();
            this.y = new ComplexType();
            this.Meh((x, y) => x.Name == y.Name && x.Value == y.Value);
        }

        [Benchmark(Baseline = true)]
        public bool this_x_Equals_this_y()
        {
            return this.x.Equals(this.y);
        }

        [Benchmark]
        public bool this_compare_this_x__this_y()
        {
            return this.compare(this.x, this.y);
        }

        private void Meh(Expression<Func<ComplexType, ComplexType, bool>> compare)
        {
            this.compare = compare.Compile();
        }
    }
}