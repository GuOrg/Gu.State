namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    public class Array : ArrayTests
    {
        public override bool EqualBy<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return State.EqualBy.PropertyValues(x, y, referenceHandling);
        }
    }
}