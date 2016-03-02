namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal static class FieldInfoExt
    {
        internal static bool IsEventField(this FieldInfo field)
        {
            return typeof(MulticastDelegate).IsAssignableFrom(field.FieldType);
        }

        internal static IReadOnlyList<FieldInfo> GetIgnoreFields(this Type type, BindingFlags bindingFlags, string[] ignoredFields)
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
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendLine($"{nameof(GetIgnoreFields)} failed. The type {type.PrettyName()} does not have the follwing field specified in {nameof(ignoredFields)}");
                foreach (var fieldName in missing)
                {
                    errorBuilder.AppendLine(fieldName);
                }

                throw new ArgumentException(errorBuilder.ToString(), nameof(ignoredFields));
            }

            return fieldInfos;
        }
    }
}