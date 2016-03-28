namespace Gu.State.Tests.DiffTests.PropertyValues
{
    using Gu.State.Tests.EqualByTests;

    public class Collections : CollectionTests
    {
        public override bool EqualByMethod<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return EqualBy.PropertyValues(x, y, referenceHandling: referenceHandling);
        }
    }
}