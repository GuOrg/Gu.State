namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    internal static partial class StringBuilderExt
    {
        internal static StringBuilder AppendSuggestFixForUnsupportedMembers(this StringBuilder errorBuilder, IErrors errors, Func<StringBuilder, MemberInfo, StringBuilder> fix)
        {
            foreach (var uf in errors.UnsupportedFields)
            {
                errorBuilder = fix(errorBuilder.CreateIfNull(), uf);
            }

            foreach (var up in errors.UnsupportedProperties)
            {
                errorBuilder = fix(errorBuilder.CreateIfNull(), up);
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestFixForUnsupportedTypes(this StringBuilder errorBuilder, IErrors errors, Func<StringBuilder, Type, StringBuilder> fix)
        {
            foreach (var ut in errors.UnsupportedTypes)
            {
                errorBuilder = fix(errorBuilder.CreateIfNull(), ut);
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestExcludeUnsupportedTypes(this StringBuilder errorBuilder, IErrors errors)
        {
            foreach (var ut in errors.UnsupportedTypes)
            {
                errorBuilder = errorBuilder.CreateIfNull()
                                           .AppendExcludeType(ut);
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestExcludeUnsupportedMembers(this StringBuilder errorBuilder, IErrors errors)
        {
            foreach (var uf in errors.UnsupportedFields)
            {
                errorBuilder = errorBuilder.CreateIfNull()
                                           .AppendExcludeField(errors.Type, uf);
            }

            foreach (var up in errors.UnsupportedProperties)
            {
                errorBuilder = errorBuilder.CreateIfNull()
                                           .AppendExcludeProperty(errors.Type, up);
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestExcludeUnsupportedIndexers(this StringBuilder errorBuilder, IErrors errors, IMemberSettings settings)
        {
            if (settings is PropertiesSettings)
            {
                foreach (var up in errors.UnsupportedIndexers)
                {
                    Debug.Assert(up.GetIndexParameters().Length > 0, "Must be an indexer");
                    errorBuilder = errorBuilder.CreateIfNull()
                                               .AppendExcludeProperty(errors.Type, up);
                }
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendUnsupportedMembers<TSettings>(this StringBuilder errorBuilder, Type type, IErrors errors)
            where TSettings : class, IMemberSettings
        {
            if (typeof(TSettings).IsAssignableFrom(typeof(PropertiesSettings)))
            {
                return errorBuilder.CreateIfNull()
                                   .AppendUnsupportedProperties(type, errors.UnsupportedProperties)
                                   .AppendUnsupportedIndexers(type, errors.UnsupportedIndexers);
            }

            if (typeof(TSettings).IsAssignableFrom(typeof(FieldsSettings)))
            {
                return errorBuilder.CreateIfNull()
                                   .AppendUnsupportedFields(type, errors.UnsupportedFields)
                                   .AppendUnsupportedIndexers(type, errors.UnsupportedIndexers);
            }

            throw Throw.ExpectedParameterOfTypes<PropertiesSettings, FieldsSettings>("{T}");
        }

        internal static StringBuilder AppendUnsupportedIndexers(this StringBuilder errorBuilder, Type type, IReadOnlyList<PropertyInfo> unSupportedIndexers)
        {
            if (unSupportedIndexers == null || unSupportedIndexers.Count == 0)
            {
                return errorBuilder;
            }

            errorBuilder = errorBuilder.CreateIfNull().AppendLine($"Indexers are not supported.");

            foreach (var property in unSupportedIndexers)
            {
                errorBuilder.AppendLine($"The property {type.PrettyName()}.{property.Name} of type {property.PropertyType.PrettyName()} is an indexer and not supported.");
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendUnsupportedProperties(this StringBuilder errorBuilder, Type type, IReadOnlyList<PropertyInfo> unSupportedProperties)
        {
            if (unSupportedProperties == null)
            {
                return errorBuilder;
            }

            foreach (var property in unSupportedProperties)
            {
                errorBuilder = errorBuilder.CreateIfNull()
                                           .AppendLine($"The property {type.PrettyName()}.{property.Name} of type {property.PropertyType.PrettyName()} is not supported.");
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendUnsupportedFields(this StringBuilder errorBuilder, Type type, IReadOnlyList<FieldInfo> unSupportedProperties)
        {
            if (unSupportedProperties == null)
            {
                return errorBuilder;
            }

            foreach (var property in unSupportedProperties)
            {
                errorBuilder = errorBuilder.CreateIfNull()
                                           .AppendLine($"The field {type.PrettyName()}.{property.Name} of type {property.FieldType.PrettyName()} is not supported.");
            }

            return errorBuilder;
        }
    }
}
