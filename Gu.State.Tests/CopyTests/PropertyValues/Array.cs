namespace Gu.State.Tests.CopyTests.PropertyValues
{
    public class Array : ArrayTests
    {
        public override void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling)
        {
            Copy.PropertyValues(source, target, referenceHandling);
        }
    }
}