namespace Gu.State.Tests.EqualByTests.FieldValues
{
    using System.Collections.Generic;

    public class CustomComparer : CustomComparerTests
    {
        public override bool EqualBy<T, TValue>(T x, T y, IEqualityComparer<TValue> comparer, ReferenceHandling referenceHandling = ReferenceHandling.Structural)
        {
            var builder = FieldsSettings.Build();
            builder.AddComparer(comparer);
            var settings = builder.CreateSettings(referenceHandling);
            return State.EqualBy.FieldValues(x, y, settings);
        }
    }
}