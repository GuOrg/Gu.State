namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static partial class Copy
    {

        //public static void VerifyCanCopyFieldValues<T>(
        //    BindingFlags bindingFlags = Constants.DefaultFieldBindingFlags,
        //    ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        //{
        //    var settings = FieldsSettings.GetOrCreate(bindingFlags, referenceHandling);
        //    VerifyCanCopyFieldValues<T>(settings);
        //}


        //public static void VerifyCanCopyFieldValues<T>(FieldsSettings settings)
        //{
        //    VerifyCanCopyFieldValues(typeof(T), settings);
        //}


        //public static void VerifyCanCopyFieldValues(Type type, FieldsSettings settings)
        //{
        //    var errorBuilder = new StringBuilder();
        //    VerifyCanCopyFieldValues(type, settings, errorBuilder, null);
        //    if (errorBuilder.Length > 0)
        //    {
        //        var message = errorBuilder.ToString();
        //        throw new NotSupportedException(message);
        //    }
        //}

        //private static void VerifyCanCopyFieldValues(
        //    Type type,
        //    FieldsSettings settings,
        //    StringBuilder errorBuilder,
        //    List<Type> checkedTypes)
        //{
        //    Verify.Enumerable(type, settings, errorBuilder);
        //    Verify.Indexers(type, settings, errorBuilder);

        //    var fieldInfos = type.GetFields(settings.BindingFlags);
        //    foreach (var fieldInfo in fieldInfos)
        //    {
        //        if (settings.IsIgnoringField(fieldInfo))
        //        {
        //            continue;
        //        }

        //        if (!IsCopyableType(fieldInfo.FieldType))
        //        {
        //            switch (settings.ReferenceHandling)
        //            {
        //                case ReferenceHandling.Throw:
        //                    Copy.AppendCannotCopyMember(errorBuilder, type, fieldInfo, settings);
        //                    break;
        //                case ReferenceHandling.References:
        //                    break;
        //                case ReferenceHandling.Structural:
        //                case ReferenceHandling.StructuralWithReferenceLoops:
        //                    if (checkedTypes == null)
        //                    {
        //                        checkedTypes = new List<Type> { type };
        //                    }

        //                    if (checkedTypes.All(x => x != fieldInfo.FieldType))
        //                    {
        //                        VerifyCanCopyFieldValues(fieldInfo.FieldType, settings, errorBuilder, checkedTypes);
        //                    }

        //                    break;
        //                default:
        //                    throw new ArgumentOutOfRangeException();
        //            }
        //        }
        //    }
        //}
    }
}
