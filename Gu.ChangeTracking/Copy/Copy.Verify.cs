namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Text;

    public static partial class Copy
    {
        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore properties for settings for</typeparam>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is used to new up references so a default constructor is required, can be private
        /// </param>
        public static void VerifyCanCopyPropertyValues<T>(
            BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            VerifyCanCopyPropertyValues<T>(settings);
        }

        /// <summary>
        /// Check if the properties of <typeparamref name="T"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <param name="settings">Contains configuration for how copy will be performed</param>
        public static void VerifyCanCopyPropertyValues<T>(PropertiesSettings settings)
        {
            var type = typeof(T);
            VerifyCanCopyPropertyValues(type, settings);
        }

        /// <summary>
        /// Check if the properties of <paramref name="type"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="settings">Contains configuration for how copy will be performed</param>
        public static void VerifyCanCopyPropertyValues(Type type, PropertiesSettings settings)
        {
            Copy.Verify.GetPropertiesErrors(type, settings)
                  .ThrowIfHasErrors(settings);
        }

        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for</typeparam>
        /// <param name="bindingFlags">The binding flags to use when getting fields</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        public static void VerifyCanCopyFieldValues<T>(
            BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var settings = FieldsSettings.GetOrCreate(bindingFlags, referenceHandling);
            VerifyCanCopyFieldValues<T>(settings);
        }

        /// <summary>
        /// Check if the fields of <typeparamref name="T"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <typeparamref name="T"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <typeparam name="T">The type to get ignore fields for settings for</typeparam>
        /// <param name="settings">Contains configuration for how copy is performed</param>
        public static void VerifyCanCopyFieldValues<T>(FieldsSettings settings)
        {
            var type = typeof(T);
            VerifyCanCopyFieldValues(type, settings);
        }

        /// <summary>
        /// Check if the fields of <paramref name="type"/> can be copied.
        /// This method will throw an exception if copy cannot be performed for <paramref name="type"/>
        /// Read the exception message for detailed instructions about what is wrong.
        /// Use this to fail fast or in unit tests.
        /// </summary>
        /// <param name="type">The type to get ignore fields for settings for</param>
        /// <param name="settings">Contains configuration for how copy is performed</param>
        public static void VerifyCanCopyFieldValues(Type type, FieldsSettings settings)
        {
            Copy.Verify.GetFieldsErrors(type, settings)
                  .ThrowIfHasErrors(settings);
        }

        private static TypeErrors CheckIsCopyableEnumerable<TSetting>(this TypeErrors typeErrors, Type type, TSetting settings)
            where TSetting : IMemberSettings
        {
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                switch (settings.ReferenceHandling)
                {
                    case ReferenceHandling.Throw:
                    case ReferenceHandling.References:
                        return typeErrors;
                    case ReferenceHandling.Structural:
                    case ReferenceHandling.StructuralWithReferenceLoops:
                        if (!IsCopyableCollectionType(type))
                        {
                            return typeErrors = typeErrors.CreateIfNull(type)
                                                     .Add(new NotCopyableCollection(type));
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return typeErrors;
        }

        internal static class Verify
        {
            internal static void CanCopyPropertyValues<T>(T x, T y, PropertiesSettings settings)
            {
                var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
                GetPropertiesErrors(type, settings)
                    .ThrowIfHasErrors(settings);
            }

            internal static TypeErrors GetPropertiesErrors(Type type, PropertiesSettings settings, MemberPath path = null)
            {
                return settings.CopyErrors.GetOrAdd(type, t => VerifyCore(settings, t)
                                                                   .CheckProperties(t, settings, path, GetPropertyErrors));
            }

            internal static void CanCopyFieldValues<T>(T x, T y, FieldsSettings settings)
            {
                var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
                GetFieldsErrors(type, settings)
                    .ThrowIfHasErrors(settings);
            }

            internal static TypeErrors GetFieldsErrors(Type type, FieldsSettings settings, MemberPath path = null)
            {
                return settings.CopyErrors.GetOrAdd(type, t => VerifyCore(settings, t)
                                                                   .CheckFields(t, settings, path, GetFieldErrors));
            }

            private static TypeErrors VerifyCore(IMemberSettings settings, Type type)
            {
                return ErrorBuilder.Start()
                                   .CheckReferenceHandlingIfEnumerable(type, settings)
                                   .CheckIsCopyableEnumerable(type, settings)
                                   .CheckIndexers(type, settings);
            }

            private static Error GetPropertyErrors(Type type, PropertyInfo property, PropertiesSettings settings, MemberPath path)
            {
                if (IsCopyableType(property.PropertyType))
                {
                    return null;
                }

                if (settings.ReferenceHandling == ReferenceHandling.References)
                {
                    return null;
                }

                if (settings.ReferenceHandling == ReferenceHandling.Throw)
                {
                    return new RequiresReferenceHandling(property.PropertyType);
                }

                return GetPropertiesErrors(property.PropertyType, settings, path);
            }

            private static Error GetFieldErrors(Type type, FieldInfo field, FieldsSettings settings, MemberPath path)
            {
                if (IsCopyableType(field.FieldType))
                {
                    return null;
                }

                if (settings.ReferenceHandling == ReferenceHandling.References)
                {
                    return null;
                }

                if (settings.ReferenceHandling == ReferenceHandling.Throw)
                {
                    return new RequiresReferenceHandling(field.FieldType);
                }

                return GetFieldsErrors(field.FieldType, settings, path);
            }

            public static void Indexers<T>(Type type, T settings) where T : IMemberSettings
            {
                var errorBuilder = Indexers(type, settings, null);
                if (errorBuilder != null)
                {
                    throw new NotSupportedException(errorBuilder.ToString());
                }
            }

            public static StringBuilder Indexers<T>(Type type, T settings, StringBuilder errorBuilder) where T : IMemberSettings
            {
                var propertyInfos = type.GetProperties(Constants.DefaultFieldBindingFlags);
                foreach (var propertyInfo in propertyInfos)
                {
                    if (propertyInfo.GetIndexParameters()
                                    .Length == 0)
                    {
                        continue;
                    }

                    if (settings.IsIgnoringDeclaringType(propertyInfo.DeclaringType))
                    {
                        continue;
                    }

                    errorBuilder = errorBuilder.CreateIfNull()
                                               .AppendCannotCopyIndexer<T>(type, propertyInfo);
                }

                return errorBuilder;
            }
        }
    }
}
