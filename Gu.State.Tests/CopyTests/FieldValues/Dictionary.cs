namespace Gu.State.Tests.CopyTests.FieldValues
{
    public class Dictionary : DictionaryTests
    {
        public override void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling)
        {
            Copy.FieldValues(source, target, referenceHandling);
        }
    }
}