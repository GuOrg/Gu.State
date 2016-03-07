namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Reflection;

    /// <summary>
    /// Defines methods for copying values from one instance to another
    /// </summary>
    public static partial class Copy
    {
        internal static bool IsCopyableType(Type type)
        {
            return type.IsImmutable();
        }

        internal static object CreateInstance<TSettings>(object sourceValue, MemberInfo member, TSettings settings)
            where TSettings : class, IMemberSettings
        {
            if (sourceValue == null)
            {
                return null;
            }

            var type = sourceValue.GetType();
            if (type.IsArray)
            {
                var constructor = type.GetConstructor(new Type[] { typeof(int) });
                var parameters = new[] { type.GetProperty("Length").GetValue(sourceValue) };
                var array = constructor.Invoke(parameters);
                return array;
            }

            if (type.IsImmutable())
            {
                return sourceValue;
            }

            try
            {
                return Activator.CreateInstance(type, true);
            }
            catch (Exception e)
            {
                throw Throw.CreateCannotCreateInstanceException(sourceValue, member, settings, e);
            }
        }

        private static bool IsCopyableCollectionType(Type type)
        {
            return typeof(IList).IsAssignableFrom(type) || typeof(IDictionary).IsAssignableFrom(type);
        }
    }
}
