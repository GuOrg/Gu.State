namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Text;

    public static partial class Copy
    {
        internal static bool IsCopyableType(Type type)
        {
            return type.IsImmutable();
        }

        internal static bool IsCopyableCollectionType(Type type)
        {
            return typeof(IList).IsAssignableFrom(type) || typeof(IDictionary).IsAssignableFrom(type);
        }

        internal static object CreateInstance<T>(object sourceValue, MemberInfo member)
            where T : CopySettings
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
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendCopyFailed<T>();
                errorBuilder.AppendLine($"{typeof(Activator).Name}.{nameof(Activator.CreateInstance)} failed for type {sourceValue.GetType() .PrettyName()}.")
                            .AppendSolveTheProblemBy()
                            .AppendLine($"* Add a parameterless constructor to {type.PrettyName()}, can be private.")
                            .AppendSuggestCopySettings<T>(type, member);
                var message = errorBuilder.ToString();
                throw new NotSupportedException(message, e);
            }
        }
    }
}
