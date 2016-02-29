namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;
    using System.Text;

    public static partial class EqualBy
    {
        private static void ThrowCannotCompareMember(Type sourceType, MemberInfo member)
        {
            var errorBuilder = new StringBuilder();
            Type memberType = null;
            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                errorBuilder.AppendLine($"EqualBy.{nameof(PropertyValues)}(x, y) does not support comparing the property {sourceType.PrettyName()}.{propertyInfo.Name}.");
                errorBuilder.AppendLine($"The property is of type {propertyInfo.PropertyType.PrettyName()}.");
                memberType = propertyInfo.PropertyType;
            }
            else
            {
                var fieldInfo = member as FieldInfo;
                if (fieldInfo != null)
                {
                    errorBuilder.AppendLine($"EqualBy.{nameof(FieldValues)}(x, y) does not support comparing the field {sourceType.PrettyName()}.{fieldInfo.Name}.");
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
            throw new NotSupportedException(errorBuilder.ToString());
        }

        private static void ThrowCannotCompareType<T>(Type type, T settings)
            where T : IEqualBySettings
        {
            var errorBuilder = new StringBuilder();
            if (settings is IEqualByPropertiesSettings)
            {
                errorBuilder.AppendLine($"EqualBy.{nameof(PropertyValues)}(x, y) does not support comparing the type {type.PrettyName()}.");
            }
            else if (settings is IEqualByFieldsSettings)
            {
                errorBuilder.AppendLine($"EqualBy.{nameof(FieldValues)}(x, y) does not support comparing the type {type.PrettyName()}.");
            }
            else
            {
                ThrowHelper.ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<IEqualByPropertiesSettings, IEqualByFieldsSettings>(nameof(settings));
            }

            errorBuilder.AppendSolveTheProblemBy()
                        .AppendSuggestImplementIEquatable(type)
                        .AppendSuggestEqualBySettings<EqualByPropertiesSettings>();
            throw new NotSupportedException(errorBuilder.ToString());
        }
    }
}
