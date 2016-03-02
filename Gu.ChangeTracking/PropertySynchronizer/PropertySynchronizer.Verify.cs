namespace Gu.ChangeTracking
{
    using System.Reflection;

    public static partial class PropertySynchronizer
    {
        public static void VerifyCanSynchronize<T>(params string[] excludedProperties)
        {
            var settings = CopyPropertiesSettings.Create(typeof(T), excludedProperties, Constants.DefaultPropertyBindingFlags, ReferenceHandling.Throw);
            VerifyCanSynchronize<T>(settings);
        }

        public static void VerifyCanSynchronize<T>(BindingFlags bindingFlags, params string[] excludedProperties)
        {
            var settings = CopyPropertiesSettings.Create(typeof(T), excludedProperties, bindingFlags, ReferenceHandling.Throw);
            VerifyCanSynchronize<T>(settings);
        }

        public static void VerifyCanSynchronize<T>(CopyPropertiesSettings settings)
        {
            ChangeTracker.VerifyCanTrack<T>(settings);
            Copy.VerifyCanCopyPropertyValues<T>(settings);
        }
    }
}
