namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class PropertyInfoExt
    {
        internal static bool IsCalculated(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.SetMethod != null)
            {
                return false;
            }

            var getMethod = propertyInfo.GetMethod;
            return Attribute.GetCustomAttribute(getMethod, typeof(CompilerGeneratedAttribute)) == null;
        }

        internal static IEnumerable<PropertyInfo> GetIgnoreProperties(this Type type, string[] ignoreProperties)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type), "Type cannot be null");
            }

            if (ignoreProperties == null || ignoreProperties.Length == 0)
            {
                return null;
            }

            var propertyInfos = type.GetProperties().Where(p => ignoreProperties.Any(x => x == p.Name)).ToArray();
            if (propertyInfos.Length != ignoreProperties.Length)
            {
                var missing = ignoreProperties.Where(x => propertyInfos.All(p => p.Name != x))
                                              .ToArray();
                throw new ArgumentException($"The type {type} does not have properties named {{{string.Join(", ", missing.Select(x => $"'{x}'"))}}}");
            }

            return propertyInfos;
        }
    }
}
