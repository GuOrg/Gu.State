namespace Gu.ChangeTracking.Tests.EqualByTests.FieldValues
{
    using System.Collections.Generic;

    public class Classes : ClassesTests
    {
        public override bool EqualMethod<T>(T x, T y)
        {
            return EqualBy.FieldValues(x, y);
        }

        public override bool EqualMethod<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return EqualBy.FieldValues(x, y, referenceHandling: referenceHandling);
        }

        public override bool EqualMethod<T>(T x, T y, params string[] excluded)
        {
            return EqualBy.FieldValues(x, y, excludedFields: excluded);
        }


        public override bool EqualMethod<T>(T x, T y, string excluded, ReferenceHandling referenceHandling)
        {
            var settings = FieldsSettings.Create(
                x,
                y,
                Constants.DefaultFieldBindingFlags,
                referenceHandling,
                new[] { excluded });
            return EqualBy.FieldValues(x, y, settings);
        }

        public static IReadOnlyList<EqualByTestsShared.EqualsData> EqualsSource => EqualByTestsShared.EqualsSource;
    }
}
