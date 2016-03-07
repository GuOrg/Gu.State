namespace Gu.State.Tests.CopyTests.FieldValues
{
    public class ReferenceLoops : CopyTests.ReferenceLoops
    {
        public override void CopyMethod<T>(
            T source,
            T target,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw,
            string excluded = null)
        {
            var builder = FieldsSettings.Build();
            if (excluded != null)
            {
                builder.AddIgnoredField<T>(excluded);
            }

            //if (excludedType != null)
            //{
            //    builder.IgnoreType(excludedType);
            //}

            var settings = builder.CreateSettings(referenceHandling);
            Copy.FieldValues(source, target, settings);
        }
    }
}