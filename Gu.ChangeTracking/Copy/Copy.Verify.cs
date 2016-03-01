namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Text;

    public static partial class Copy
    {
        private static class Verify
        {
            public static void Enumerable<T>(Type type, T settings, StringBuilder errorBuilder)
                where T : CopySettings
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    switch (settings.ReferenceHandling)
                    {
                        case ReferenceHandling.Throw:
                            Throw.AppendCannotCopyType(errorBuilder, type, settings);
                            break;
                        case ReferenceHandling.References:
                        case ReferenceHandling.Structural:
                        case ReferenceHandling.StructuralWithReferenceLoops:
                            if (!IsCopyableCollectionType(type))
                            {
                                Throw.AppendCannotCopyType(errorBuilder, type, settings);
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            public static void Indexers<T>(Type type, T settings)
                        where T : CopySettings
            {
                var errorBuilder = Indexers(type, settings, null);
                if (errorBuilder != null)
                {
                    throw new NotSupportedException(errorBuilder.ToString());
                }
            }

            public static StringBuilder Indexers<T>(Type type, T settings, StringBuilder errorBuilder)
                where T : CopySettings
            {
                var propertyInfos = type.GetProperties(Constants.DefaultFieldBindingFlags);
                foreach (var propertyInfo in propertyInfos)
                {
                    if (propertyInfo.GetIndexParameters().Length == 0)
                    {
                        continue;
                    }

                    if (settings.IsIgnoringType(propertyInfo.DeclaringType))
                    {
                        continue;
                    }

                    if (errorBuilder == null)
                    {
                        errorBuilder = new StringBuilder();
                    }

                    Throw.AppendCannotCopyIndexer<T>(errorBuilder, type, propertyInfo);
                }

                return errorBuilder;
            }
        }
    }
}
