namespace Gu.State
{
    public static partial class DiffBy
    {
        private static string DiffMethodName(this MemberSettings settings)
        {
            if (settings is PropertiesSettings)
            {
                return nameof(PropertyValues);
            }

            if (settings is FieldsSettings)
            {
                return nameof(FieldValues);
            }

            throw Throw.ExpectedParameterOfTypes<FieldsSettings, PropertiesSettings>("DiffMethodName failed.");
        }

        private static bool TryGetValueDiff(object x, object y, MemberSettings settings, out ValueDiff diff)
        {
            if (EqualBy.TryGetValueEquals(x, y, settings, out bool result))
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
