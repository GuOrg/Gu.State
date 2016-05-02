namespace Gu.State.Tests.EqualByTests.FieldValues
{
    using System.Collections.Generic;

    public class CustomComparer : CustomComparerTests
    {
        public override bool EqualMethod<T, TValue>(T x, T y, IEqualityComparer<TValue> comparer, ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var builder = FieldsSettings.Build();
            builder.AddComparer(comparer);
            var settings = builder.CreateSettings(referenceHandling);
            return EqualBy.FieldValues(x, y, settings);
        }
    }
}