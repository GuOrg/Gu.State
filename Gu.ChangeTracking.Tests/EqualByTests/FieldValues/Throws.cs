namespace Gu.ChangeTracking.Tests.EqualByTests.FieldValues
{
    using Gu.ChangeTracking.Tests.EqualByTests;

    public class Throws : ThrowsTests
    {
        public override bool EqualByMethod<T>(T x, T y)
        {
            return EqualBy.FieldValues(x, y);
        }

        public override bool EqualByMethod<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return EqualBy.FieldValues(x, y, referenceHandling: referenceHandling);
        }
    }
}
