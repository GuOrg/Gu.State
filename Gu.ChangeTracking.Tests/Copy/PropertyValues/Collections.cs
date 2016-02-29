namespace Gu.ChangeTracking.Tests.PropertyValues
{
    public class Collections : Gu.ChangeTracking.Tests.CollectionTests
    {
        public override void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling)
        {
            Copy.PropertyValues(source, target, referenceHandling);
        }
    }
}