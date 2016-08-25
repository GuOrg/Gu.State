namespace Gu.State.Benchmarks
{
    using System;
    using BenchmarkDotNet.Attributes;

    public class EqualBy
    {
        private readonly ComplexType x = new ComplexType();
        private readonly ComplexType y = new ComplexType();
        private readonly Func<ComplexType, ComplexType, bool> compareFunc = (x, y) => x.Name == y.Name && x.Value == y.Value;

        [Benchmark(Baseline = true)]
        public bool this_x_Equals_this_y()
        {
            return this.x.Equals(this.y);
        }

        [Benchmark]
        public bool ObjectEquals()
        {
            return Equals(this.x, this.y);
        }

        [Benchmark]
        public bool Func()
        {
            return this.compareFunc(this.x, this.y);
        }

        [Benchmark]
        public bool EqualByPropertyValues()
        {
            return State.EqualBy.PropertyValues(this.x, this.y);
        }

        [Benchmark]
        public bool EqualByFieldValues()
        {
            return State.EqualBy.FieldValues(this.x, this.y);
        }
    }
}