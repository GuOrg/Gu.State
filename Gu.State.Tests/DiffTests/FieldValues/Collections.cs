namespace Gu.State.Tests.DiffTests.FieldValues
{

    public class Collections : CollectionTests
    {
        public override Diff DiffMethod<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return DiffBy.FieldValues(x, y, referenceHandling: referenceHandling);
        }
    }
}