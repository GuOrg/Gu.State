namespace Gu.State.Tests.EqualByTests.FieldValues
{
    public class Enumerable : EnumerableTests
    {
        public override bool EqualByMethod<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return EqualBy.FieldValues(x, y, referenceHandling);
        }
    }
}