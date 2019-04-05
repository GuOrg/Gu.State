namespace Gu.State.Tests.CopyTests.FieldValues
{
    public class ReferenceLoops : CopyTests.ReferenceLoops
    {
        public override void CopyMethod<T>(
            T source,
            T target,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            string excluded = null)
        {
            var builder = FieldsSettings.Build();
            if (excluded != null)
            {
                _ = builder.AddIgnoredField<T>(excluded);
            }

            var settings = builder.CreateSettings(referenceHandling);
            Copy.FieldValues(source, target, settings);
        }
    }
}