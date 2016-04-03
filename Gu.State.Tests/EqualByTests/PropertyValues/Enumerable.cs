namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    public class Enumerable : EnumerableTests
    {
        public override bool EqualByMethod<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return EqualBy.PropertyValues(x, y, referenceHandling: referenceHandling);
        }
    }
}