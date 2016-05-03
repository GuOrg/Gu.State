namespace Gu.State
{
    public static partial class DiffBy
    {
        private static string DiffMethodName(this IMemberSettings settings)
        {
            if (settings is PropertiesSettings)
            {
                return nameof(PropertyValues);
            }

            if (settings is FieldsSettings)
            {
                return nameof(FieldValues);
            }

            throw State.Throw.ExpectedParameterOfTypes<FieldsSettings, PropertiesSettings>("DiffMethodName failed.");
        }

        private static bool TryGetValueDiff<TSettings>(object x, object y, TSettings settings, out ValueDiff diff)
            where TSettings : IMemberSettings
        {
            bool result;
            if (EqualBy.TryGetValueEquals(x, y, settings, out result))
            {
                diff = result
                           ? null
                           : new ValueDiff(x, y);

                return true;
            }

            diff = null;
            return false;
        }
    }
}
