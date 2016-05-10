namespace Gu.State.Tests.EqualByTests.FieldValues
{
    public class Dictionary : DictionaryTests
    {
        public override bool EqualByMethod<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return EqualBy.FieldValues(x, y, referenceHandling);
        }
    }
}