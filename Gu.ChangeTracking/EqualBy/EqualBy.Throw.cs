namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;
    using System.Text;

    public static partial class EqualBy
    {
        private static class Throw
        {
            internal static void CannotCompareMember(Type sourceType, MemberInfo member)
            {
                var errorBuilder = new StringBuilder();
                Type memberType = null;
                var propertyInfo = member as PropertyInfo;
                if (propertyInfo != null)
                {
                    errorBuilder.AppendEqualByFailed<EqualByPropertiesSettings>();
                    errorBuilder.AppendLine($"The property {sourceType.PrettyName()}.{propertyInfo.Name} is not supported.");
                    errorBuilder.AppendLine($"The property is of type {propertyInfo.PropertyType.PrettyName()}.");
                    memberType = propertyInfo.PropertyType;
                }
                else
                {
                    var fieldInfo = member as FieldInfo;
                    if (fieldInfo != null)
                    {
                        errorBuilder.AppendEqualByFailed<EqualByFieldsSettings>();
                        errorBuilder.AppendLine($"The field {sourceType.PrettyName()}.{fieldInfo.Name} is not supported.");
                        errorBuilder.AppendLine($"The field is of type {fieldInfo.FieldType.PrettyName()}.");
                        memberType = fieldInfo.FieldType;
                    }
                    else
                    {
                        ThrowHelper.ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<PropertyInfo, FieldInfo>(nameof(member));
                    }
                }

                errorBuilder.AppendSolveTheProblemBy()
                            .AppendSuggestImplementIEquatable(memberType)
                            .AppendSuggestEqualBySettings<EqualByFieldsSettings>();
                var message = errorBuilder.ToString();
                throw new NotSupportedException(message);
            }

            // ReSharper disable once UnusedParameter.Local
            internal static void CannotCompareType<T>(Type type, T settings) 
                where T : IEqualBySettings
            {
                CannotCompareType<T>(type);
            }

            internal static void CannotCompareType<T>(Type type)
                where T : IEqualBySettings
            {
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendEqualByFailed<T>()
                            .AppendSolveTheProblemBy()
                            .AppendSuggestImplementIEquatable(type)
                            .AppendSuggestEqualBySettings<EqualByPropertiesSettings>();
                throw new NotSupportedException(errorBuilder.ToString());
            }
        }

        private static StringBuilder AppendEqualByFailed<T>(this StringBuilder errorBuilder)
                    where T : IEqualBySettings
        {
            if (typeof(IEqualByPropertiesSettings).IsAssignableFrom(typeof(T)))
            {
                errorBuilder.AppendLine($"EqualBy.{nameof(PropertyValues)}(x, y) failed.");
            }
            else if (typeof(IEqualByFieldsSettings).IsAssignableFrom(typeof(T)))
            {
                errorBuilder.AppendLine($"EqualBy.{nameof(FieldValues)}(x, y) failed.");
            }
            else
            {
                ThrowHelper.ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<IEqualByPropertiesSettings, IEqualByFieldsSettings>("T");
            }

            return errorBuilder;
        }
    }
}
