namespace Gu.State.Tests.CopyTests.FieldValues
{
    public class Array : ArrayTests
    {
        public override void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling)
        {
            Copy.FieldValues(source, target, referenceHandling);
        }
    }
}