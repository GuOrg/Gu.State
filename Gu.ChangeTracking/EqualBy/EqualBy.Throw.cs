﻿namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;
    using System.Text;

    public static partial class EqualBy
    {
        internal static StringBuilder AppendSuggestImplementIEquatable(this StringBuilder errorBuilder, Type sourceType)
        {
            return errorBuilder.AppendSuggestImplement(sourceType, $"IEquatable<{sourceType.PrettyName()}>");
        }

        private static StringBuilder AppendEqualByFailed<T>(this StringBuilder errorBuilder)
            where T : class, IMemberSettings
        {
            if (typeof(PropertiesSettings).IsAssignableFrom(typeof(T)))
            {
                errorBuilder.AppendLine($"EqualBy.{nameof(PropertyValues)}(x, y) failed.");
            }
            else if (typeof(FieldsSettings).IsAssignableFrom(typeof(T)))
            {
                errorBuilder.AppendLine($"EqualBy.{nameof(FieldValues)}(x, y) failed.");
            }
            else
            {
                throw Gu.ChangeTracking.Throw.ExpectedParameterOfTypes<PropertiesSettings, FieldsSettings>("T");
            }

            return errorBuilder;
        }

        private static StringBuilder AppendSuggestImplementIEquatable(this StringBuilder errorBuilder, MemberInfo member)
        {
            return errorBuilder.AppendSuggestImplementIEquatable(member.MemberType());
        }

        private static void ThrowIfHasErrors<TSetting>(this TypeErrors errors, Type type, TSetting settings)
            where TSetting : class, IMemberSettings
        {
            if (errors == null)
            {
                return;
            }

            var errorBuilder = new StringBuilder();
            errorBuilder.AppendEqualByFailed<TSetting>()
                        .AppendNotSupported(errors)
                        .AppendSolveTheProblemBy()
                        .AppendSuggestEquatable(errors)
                        .AppendLine($"* Use {typeof(TSetting).Name} and specify how comparing is performed:")
                        .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Structural)} means that a deep equals is performed.")
                        .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.StructuralWithReferenceLoops)} means that a deep equals that handles reference loops is performed.")
                        .AppendLine($"  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.References)} means that reference equality is used.")
                        .AppendSuggestExclude(errors);

            var message = errorBuilder.ToString();
            throw new NotSupportedException(message);
        }
    }
}
