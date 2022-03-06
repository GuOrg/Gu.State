namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    using System.Collections.Generic;

    public class CustomComparer : CustomComparerTests
    {
        public override bool EqualBy<T, TValue>(T x, T y, IEqualityComparer<TValue> comparer, ReferenceHandling referenceHandling = ReferenceHandling.Structural)
        {
            var settings = PropertiesSettings.Build()
                                             .AddComparer(comparer)
                                             .CreateSettings(referenceHandling);
            return State.EqualBy.PropertyValues(x, y, settings);
        }
    }
}
