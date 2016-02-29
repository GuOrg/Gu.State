namespace Gu.ChangeTracking.Tests.CopyTests.PropertyValues
{
    public class Collections : CollectionTests
    {
        public override void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling)
        {
            Copy.PropertyValues(source, target, referenceHandling);
        }
    }
}