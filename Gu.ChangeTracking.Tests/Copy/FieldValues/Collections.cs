namespace Gu.ChangeTracking.Tests.FieldValues
{
    public class Collections : Gu.ChangeTracking.Tests.CollectionTests
    {
        public override void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling)
        {
            Copy.FieldValues(source, target, referenceHandling);
        }
    }
}