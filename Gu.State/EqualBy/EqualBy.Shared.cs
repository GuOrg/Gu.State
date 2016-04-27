namespace Gu.State
{
    /// <summary>
    /// Defines methods for comparing two instances
    /// </summary>
    public static partial class EqualBy
    {
        public static bool MemberValues(object x, object y, IMemberSettings settings)
        {
            var propertiesSettings = settings as PropertiesSettings;
            if (propertiesSettings != null)
            {
                return EqualBy.PropertyValues(x, y, propertiesSettings);
            }

            var fieldsSettings = settings as FieldsSettings;
            if (fieldsSettings != null)
            {
                return EqualBy.FieldValues(x, y, fieldsSettings);
            }

            throw Throw.ExpectedParameterOfTypes<PropertiesSettings, FieldsSettings>(nameof(settings));
        }
    }
}
