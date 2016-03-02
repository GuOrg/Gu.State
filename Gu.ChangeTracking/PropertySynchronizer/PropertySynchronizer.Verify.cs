namespace Gu.ChangeTracking
{
    using System.Reflection;

    public static partial class PropertySynchronizer
    {
        public static void VerifyCanSynchronize<T>(params string[] excludedProperties)
        {
            Copy.VerifyCanCopyPropertyValues<T>(excludedProperties);
        }

        public static void VerifyCanSynchronize<T>(BindingFlags bindingFlags, params string[] excludedProperties)
        {
            Copy.VerifyCanCopyPropertyValues<T>(bindingFlags, excludedProperties);
        }

        public static void VerifyCanSynchronize<T>(CopyPropertiesSettings settings)
        {
            Copy.VerifyCanCopyPropertyValues<T>(settings);
        }
    }
}
