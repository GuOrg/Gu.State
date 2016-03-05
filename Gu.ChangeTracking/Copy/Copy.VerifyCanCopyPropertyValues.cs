namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    //public static partial class Copy
    //{

    //    public static void VerifyCanCopyPropertyValues<T>(
    //        BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags,
    //        ReferenceHandling referenceHandling = ReferenceHandling.Throw)
    //    {
    //        var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
    //        VerifyCanCopyPropertyValues<T>(settings);
    //    }


    //    public static void VerifyCanCopyPropertyValues<T>(PropertiesSettings settings)
    //    {
    //        VerifyCanCopyPropertyValues(typeof(T), settings);
    //    }


    //    public static void VerifyCanCopyPropertyValues(Type type, PropertiesSettings settings)
    //    {
    //        var errorBuilder = new StringBuilder();
    //        VerifyCanCopyPropertyValues(type, settings, errorBuilder, null);

    //        if (errorBuilder.Length > 0)
    //        {
    //            var message = errorBuilder.ToString();
    //            throw new NotSupportedException(message);
    //        }
    //    }

    //    private static void VerifyCanCopyPropertyValues(
    //        Type type,
    //        PropertiesSettings settings,
    //        StringBuilder errorBuilder,
    //        List<Type> checkedTypes)
    //    {
    //        Verify.Enumerable(type, settings, errorBuilder);
    //        Verify.Indexers(type, settings, errorBuilder);
    //        var propertyInfos = type.GetProperties(settings.BindingFlags);
    //        foreach (var propertyInfo in propertyInfos)
    //        {
    //            if (settings.IsIgnoringProperty(propertyInfo) ||
    //                settings.GetSpecialCopyProperty(propertyInfo) != null)
    //            {
    //                continue;
    //            }

    //            if (!IsCopyableType(propertyInfo.PropertyType))
    //            {
    //                switch (settings.ReferenceHandling)
    //                {
    //                    case ReferenceHandling.Throw:
    //                        errorBuilder.AppendCannotCopyMember(type, propertyInfo, settings);
    //                        break;
    //                    case ReferenceHandling.References:
    //                        break;
    //                    case ReferenceHandling.Structural:
    //                    case ReferenceHandling.StructuralWithReferenceLoops:
    //                        if (checkedTypes == null)
    //                        {
    //                            checkedTypes = new List<Type> { type };
    //                        }

    //                        if (checkedTypes.All(x => x != propertyInfo.PropertyType))
    //                        {
    //                            VerifyCanCopyPropertyValues(propertyInfo.PropertyType, settings, errorBuilder, checkedTypes);
    //                        }

    //                        break;
    //                    default:
    //                        throw new ArgumentOutOfRangeException();
    //                }
    //            }
    //        }
    //    }
    //}
}
