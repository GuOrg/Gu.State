namespace Gu.State.Tests.DiffTests.FieldValues
{
    public class Set : SetTests
    {
        public override Diff DiffMethod<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return DiffBy.FieldValues(x, y, referenceHandling: referenceHandling);
        }
    }
}