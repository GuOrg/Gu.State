namespace Gu.State.Tests.DiffTests.PropertyValues
{
    public class Enumerable : EnumerableTests
    {
        public override Diff DiffMethod<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return DiffBy.PropertyValues(x, y, referenceHandling);
        }
    }
}