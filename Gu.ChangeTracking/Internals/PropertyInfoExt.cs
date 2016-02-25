namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

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

        internal static bool IsGetReadOnly(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.SetMethod != null)
            {
                return false;
            }

            var getMethod = propertyInfo.GetMethod;
            return Attribute.GetCustomAttribute(getMethod, typeof(CompilerGeneratedAttribute)) != null;
        }

        internal static IReadOnlyList<PropertyInfo> GetIgnoreProperties(this Type type, BindingFlags bindingFlags, string[] ignoreProperties)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type), "Type cannot be null");
            }

            if (ignoreProperties == null || ignoreProperties.Length == 0)
            {
                return null;
            }

            var propertyInfos = type.GetProperties(bindingFlags).Where(p => ignoreProperties.Any(x => x == p.Name)).ToArray();
            if (propertyInfos.Length != ignoreProperties.Length)
            {
                var missing = ignoreProperties.Where(x => propertyInfos.All(p => p.Name != x))
                                              .ToArray();
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendLine($"{nameof(GetIgnoreProperties)} failed. The type {type.PrettyName()} does not have the follwing field specified in {nameof(ignoreProperties)}");
                foreach (var name in missing)
                {
                    errorBuilder.AppendLine(name);
                }

                throw new ArgumentException(errorBuilder.ToString(), nameof(ignoreProperties));
            }

            return propertyInfos;
        }
    }
}
