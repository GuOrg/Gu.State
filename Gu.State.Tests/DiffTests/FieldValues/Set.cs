namespace Gu.State.Tests.DiffTests.FieldValues
{
    public class Set : SetTests
    {
        public override Diff DiffBy<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return State.DiffBy.FieldValues(x, y, referenceHandling);
        }
    }
}
