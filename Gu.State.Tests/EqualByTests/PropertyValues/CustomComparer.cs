namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    using System.Collections.Generic;

    public class CustomComparer : CustomComparerTests
    {
        public override bool EqualMethod<T, TValue>(T x, T y, IEqualityComparer<TValue> comparer , ReferenceHandling referenceHandling = ReferenceHandling.Structural)
        {
            var builder = PropertiesSettings.Build();
            builder.AddComparer(comparer);
            var settings = builder.CreateSettings(referenceHandling);
            return EqualBy.PropertyValues(x, y, settings);
        }
    }
}