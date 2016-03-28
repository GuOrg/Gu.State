namespace Gu.State.Tests.DiffTests.FieldValues
{
    using Gu.State.Tests.EqualByTests;

    public class Collections : CollectionTests
    {
        public override bool EqualByMethod<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return EqualBy.FieldValues(x, y, referenceHandling: referenceHandling);
        }
    }
}