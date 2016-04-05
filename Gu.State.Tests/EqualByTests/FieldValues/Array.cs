namespace Gu.State.Tests.EqualByTests.FieldValues
{
    public class Array : ArrayTests
    {
        public override bool EqualByMethod<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return EqualBy.FieldValues(x, y, referenceHandling: referenceHandling);
        }
    }
}