#pragma warning disable SA1300 // Element must begin with upper-case letter
namespace Gu.State.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using BenchmarkDotNet.Attributes;

    public class EqualByComplexType
    {
        private readonly ComplexType x = new ComplexType();
        private readonly ComplexType y = new ComplexType();
        private readonly Func<ComplexType, ComplexType, bool> compareFunc = (x, y) => x.Name == y.Name && x.Value == y.Value;
        private readonly PropertiesSettings propertiesSettings = PropertiesSettings.Build().AddComparer(ComplexTypeComparer.Default).CreateSettings();
        private readonly FieldsSettings fieldsSettings = FieldsSettings.Build().AddComparer(ComplexTypeComparer.Default).CreateSettings();

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
        public bool Comparer()
        {
            return ComplexTypeComparer.Default.Equals(this.x, this.y);
        }

        [Benchmark]
        public bool EqualByPropertyValues()
        {
            return State.EqualBy.PropertyValues(this.x, this.y);
        }

        [Benchmark]
        public bool EqualByPropertyValuesWithComparer()
        {
            return State.EqualBy.PropertyValues(this.x, this.y, this.propertiesSettings);
        }

        [Benchmark]
        public bool EqualByFieldValues()
        {
            return State.EqualBy.FieldValues(this.x, this.y);
        }

        [Benchmark]
        public bool EqualByFieldValuesWIthComparer()
        {
            return State.EqualBy.FieldValues(this.x, this.y, this.fieldsSettings);
        }

        private class ComplexTypeComparer : IEqualityComparer<ComplexType>
        {
            public static readonly ComplexTypeComparer Default = new ComplexTypeComparer();

            public bool Equals(ComplexType x, ComplexType y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return x.Value == y.Value && x.Name == y.Name;
            }

            public int GetHashCode(ComplexType obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}