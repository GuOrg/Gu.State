namespace Gu.State.Tests.DiffTests.FieldValues
{
    public class Dictionary : DictionaryTests
    {
        public override Diff DiffBy<T>(T x, T y, ReferenceHandling referenceHandling)
        {
            return DiffBy.FieldValues(x, y, referenceHandling);
        }
    }
}