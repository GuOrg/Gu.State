namespace Gu.State.Tests.EqualByTests.FieldValues
{
    public class Collections : CollectionTests
    {
        public override bool EqualBy<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return State.EqualBy.FieldValues(x, y, referenceHandling);
        }
    }
}
