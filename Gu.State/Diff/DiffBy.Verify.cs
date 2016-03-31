namespace Gu.State
{
    using System.Reflection;

    public static partial class DiffBy
    {
        public static void VerifyCanDiffByFieldValues<T>(
            ReferenceHandling referenceHandling = ReferenceHandling.Throw,
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags)
        {
            var settings = FieldsSettings.GetOrCreate(bindingFlags, referenceHandling);
            VerifyCanDiffByFieldValues<T>(settings);
        }

        public static void VerifyCanDiffByFieldValues<T>(FieldsSettings settings)
        {
            EqualBy.VerifyCanEqualByFieldValues<T>(settings, typeof(DiffBy).Name, nameof(FieldValues));
        }

        public static void VerifyCanDiffByPropertyValues<T>(
            ReferenceHandling referenceHandling = ReferenceHandling.Throw,
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
        {
            var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            VerifyCanDiffByPropertyValues<T>(settings);
        }

        public static void VerifyCanDiffByPropertyValues<T>(PropertiesSettings settings)
        {
            EqualBy.VerifyCanEqualByPropertyValues<T>(settings, typeof(DiffBy).Name, nameof(DiffBy.PropertyValues));
        }
    }
}
