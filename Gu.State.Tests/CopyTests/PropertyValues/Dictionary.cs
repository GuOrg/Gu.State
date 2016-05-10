namespace Gu.State.Tests.CopyTests.PropertyValues
{
    public class Dictionary : DictionaryTests
    {
        public override void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling)
        {
            Copy.PropertyValues(source, target, referenceHandling);
        }
    }
}