namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class FieldInfoExt
    {
        internal static bool IsEventField(this FieldInfo field)
        {
            return typeof(MulticastDelegate).IsAssignableFrom(field.FieldType);
        }

        internal static IEnumerable<FieldInfo> GetIgnoreFields(this Type type, BindingFlags bindingFlags, string[] ignoredFields)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type), "Type cannot be null");
            }

            if (ignoredFields == null || ignoredFields.Length == 0)
            {
                return null;
            }

            var fieldInfos = type.GetFields(bindingFlags).Where(p => ignoredFields.Any(x => x == p.Name)).ToArray();
            if (fieldInfos.Length != ignoredFields.Length)
            {
                var missing = ignoredFields.Where(x => fieldInfos.All(p => p.Name != x))
                                              .ToArray();
                throw new ArgumentException($"The type {type} does not have fields named {{{string.Join(", ", missing.Select(x => $"'{x}'"))}}}");
            }

            return fieldInfos;
        }
    }
}