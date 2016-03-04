namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal static partial class StringBuilderExt
    {
        internal static StringBuilder AppendSuggestFixFor<TError>(this StringBuilder errorBuilder, TypeErrors errors, Func<StringBuilder, TError, StringBuilder> fix)
            where TError : Error
        {
            foreach (var up in errors.Errors.OfType<TError>())
            {
                errorBuilder = fix(errorBuilder.CreateIfNull(), up);
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestExcludeTypes(this StringBuilder errorBuilder, TypeErrors errors)
        {
            var types = errors.Errors.OfType<TypeErrors>().Select(x => x.Type);
            var memberTypes = errors.Errors.OfType<MemberError>().Select(x => x.MemberInfo.DeclaringType).Where(t => t != null);
            types = types.Concat(memberTypes)
                         .Distinct();
            foreach (var type in types)
            {
                errorBuilder = errorBuilder.CreateIfNull()
                                           .AppendExcludeType(type);
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendSuggestExclude(this StringBuilder errorBuilder, TypeErrors errors, IMemberSettings settings)
        {
            foreach (var error in errors.OfType<TypeError>())
            {
                error.AppendSuggestExclude(errorBuilder);
            }

            foreach (var error in errors.OfType<MemberError>())
            {
                error.AppendSuggestExclude(errorBuilder);
            }

            foreach (var error in errors.OfType<UnsupportedIndexer>())
            {
                error.AppendSuggestExclude(errorBuilder, settings);
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendErrors(this StringBuilder errorBuilder, TypeErrors errors)
        {
            foreach (var error in errors.OfType<MemberError>())
            {
                error.AppendNotSupported(errorBuilder);
            }

            if (errors.OfType<UnsupportedIndexer>()
                      .Any())
            {
                errorBuilder.AppendLine("Indexers are not supported.");
                foreach (var error in errors.OfType<UnsupportedIndexer>())
                {
                    error.AppendNotSupported(errorBuilder);
                }
            }

            return errorBuilder;
        }

        internal static StringBuilder AppendUnsupportedIndexers(this StringBuilder errorBuilder, Type type, IEnumerable<PropertyInfo> unSupportedIndexers)
        {
            if (unSupportedIndexers == null || !unSupportedIndexers.Any())
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

        private static StringBuilder AppendUnsupportedProperties(this StringBuilder errorBuilder, TypeErrors errors)
        {
            throw new NotImplementedException("message");
            //foreach (var memberErrors in errors.Errors)
            //{
            //    var propertyInfo = (PropertyInfo)memberErrors.MemberInfo;
            //    errorBuilder = errorBuilder.CreateIfNull()
            //                               .AppendLine($"The property {errors.Type.PrettyName()}.{propertyInfo.Name} of type {propertyInfo.PropertyType.PrettyName()} is not supported.");
            //}

            //return errorBuilder;
        }

        private static StringBuilder AppendUnsupportedFields(this StringBuilder errorBuilder, Type type, IEnumerable<FieldInfo> unSupportedProperties)
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
