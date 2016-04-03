namespace Gu.State.Tests.DiffTests.PropertyValues
{
    public class Set : SetTests
    {
        public override Diff DiffMethod<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return DiffBy.PropertyValues(x, y, referenceHandling: referenceHandling);
        }
    }
}