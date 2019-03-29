namespace Gu.State.Tests.DiffTests.PropertyValues
{
    public class Array : ArrayTests
    {
        public override Diff DiffBy<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return State.DiffBy.PropertyValues(x, y, referenceHandling);
        }
    }
}