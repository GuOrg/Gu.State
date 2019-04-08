// ReSharper disable RedundantArgumentDefaultValue
#pragma warning disable SA1300 // Element must begin with upper-case letter
#pragma warning disable CA1822 // Mark members as static
namespace Gu.State.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using BenchmarkDotNet.Attributes;

    [MemoryDiagnoser]
    public class EqualByComplexType
    {
        private static readonly ComplexType X = new ComplexType();
        private static readonly ComplexType Y = new ComplexType();
        private static readonly Func<ComplexType, ComplexType, bool> CompareFunc = (x, y) => x.Name == y.Name && x.Value == y.Value;
        private static readonly PropertiesSettings PropertiesSettingsWithComparer = PropertiesSettings.Build().AddComparer(ComplexTypeComparer.Default).CreateSettings();
        private static readonly FieldsSettings FieldsSettingsWithComparer = FieldsSettings.Build().AddComparer(ComplexTypeComparer.Default).CreateSettings();

        [Benchmark(Baseline = true)]
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public bool this_x_Equals_this_y() => X.Equals(Y);
#pragma warning restore CA1707 // Identifiers should not contain underscores

        [Benchmark]
        public bool ObjectEquals() => Equals(X, Y);

        [Benchmark]
        public bool Func() => CompareFunc(X, Y);

        [Benchmark]
        public bool Comparer() => ComplexTypeComparer.Default.Equals(X, Y);

        [Benchmark]
        public bool EqualByPropertyValuesStructural() => EqualBy.PropertyValues(X, Y, ReferenceHandling.Structural);

        [Benchmark]
        public bool EqualByPropertyValuesReferences() => EqualBy.PropertyValues(X, Y, ReferenceHandling.References);

        [Benchmark]
        public bool EqualByPropertyValuesWithComparer() => EqualBy.PropertyValues(X, Y, PropertiesSettingsWithComparer);

        [Benchmark]
        public bool EqualByFieldValuesStructural() => EqualBy.FieldValues(X, Y, ReferenceHandling.Structural);

        [Benchmark]
        public bool EqualByFieldValuesReferences() => EqualBy.FieldValues(X, Y, ReferenceHandling.References);

        [Benchmark]
        public bool EqualByFieldValuesWithComparer() => EqualBy.FieldValues(X, Y, FieldsSettingsWithComparer);

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