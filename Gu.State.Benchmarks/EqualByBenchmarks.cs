﻿namespace Gu.State.Benchmarks
{
    using BenchmarkDotNet.Attributes;

    public class EqualByComplexTypeBenchmarks
    {
        private readonly ComplexType x;
        private readonly ComplexType y;

        public EqualByComplexTypeBenchmarks()
        {
            this.x = new ComplexType();
            this.y = new ComplexType();
        }

        [Benchmark(Baseline = true)]
        public bool ObjectEquals()
        {
            return Equals(this.x, this.y);
        }

        [Benchmark]
        public bool EqualByPropertyValues()
        {
            return EqualBy.PropertyValues(this.x, this.y);
        }

        [Benchmark]
        public bool EqualByFieldValues()
        {
            return EqualBy.FieldValues(this.x, this.y);
        }
    }
}