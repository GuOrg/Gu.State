// ReSharper disable RedundantArgumentDefaultValue
#pragma warning disable SA1300 // Element must begin with upper-case letter
namespace Gu.State.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using BenchmarkDotNet.Attributes;

    public class EqualByComplexType
    {
        private static readonly ComplexType X = new ComplexType();
        private static readonly ComplexType Y = new ComplexType();
        private static readonly Func<ComplexType, ComplexType, bool> CompareFunc = (x, y) => x.Name == y.Name && x.Value == y.Value;
        private static readonly PropertiesSettings PropertiesSettingsWithComparer = PropertiesSettings.Build().AddComparer(ComplexTypeComparer.Default).CreateSettings();
        private static readonly FieldsSettings FieldsSettingsWithComparer = FieldsSettings.Build().AddComparer(ComplexTypeComparer.Default).CreateSettings();

        [Benchmark(Baseline = true)]
        public bool this_x_Equals_this_y() => X.Equals(Y);

        [Benchmark]
        public bool ObjectEquals() => Equals(X, Y);

        [Benchmark]
        public bool Func() => CompareFunc(X, Y);

        [Benchmark]
        public bool Comparer() => ComplexTypeComparer.Default.Equals(X, Y);

        [Benchmark]
        public bool EqualByPropertyValuesStructural() => State.EqualBy.PropertyValues(X, Y, ReferenceHandling.Structural);

        [Benchmark]
        public bool EqualByPropertyValuesReferences() => State.EqualBy.PropertyValues(X, Y, ReferenceHandling.References);

        [Benchmark]
        public bool EqualByPropertyValuesWithComparer() => State.EqualBy.PropertyValues(X, Y, PropertiesSettingsWithComparer);

        [Benchmark]
        public bool EqualByFieldValuesStructural() => State.EqualBy.FieldValues(X, Y, ReferenceHandling.Structural);

        [Benchmark]
        public bool EqualByFieldValuesReferences() => State.EqualBy.FieldValues(X, Y, ReferenceHandling.References);

        [Benchmark]
        public bool EqualByFieldValuesWithComparer() => State.EqualBy.FieldValues(X, Y, FieldsSettingsWithComparer);

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

            public int GetHashCode(ComplexType obj) => throw new NotImplementedException();
        }
    }
}