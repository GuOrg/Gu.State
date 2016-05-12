namespace Gu.State
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Defines methods for copying values from one instance to another
    /// </summary>
    public static partial class Copy
    {
        internal static object CreateInstance<TSettings>(object sourceValue, TSettings settings)
            where TSettings : class, IMemberSettings
        {
            if (sourceValue == null)
            {
                return null;
            }

            var type = sourceValue.GetType();
            if (type.IsArray)
            {
                var constructor = type.GetConstructor(new[] { typeof(int) });
                var parameters = new[] { type.GetProperty("Length").GetValue(sourceValue) };
                var array = constructor.Invoke(parameters);
                return array;
            }

            if (settings.IsImmutable(type))
            {
                return sourceValue;
            }

            try
            {
                return Activator.CreateInstance(type, true);
            }
            catch (Exception e)
            {
                throw Throw.CreateCannotCreateInstanceException(sourceValue, settings, e);
            }
        }

        internal static T CloneAndSync<T, TSettings>(
            T sourceItem,
            T targetItem,
            TSettings settings,
            ReferencePairCollection referencePairs,
            bool isImmutable)
            where TSettings : class, IMemberSettings
        {
            if (sourceItem == null ||
                settings.ReferenceHandling == ReferenceHandling.References ||
                isImmutable ||
                ReferenceEquals(sourceItem, targetItem))
            {
                return sourceItem;
            }

            T clone;
            if (TryCloneWithoutSync(sourceItem, targetItem, settings, out clone))
            {
                Sync(sourceItem, clone, settings, referencePairs);
            }

            return clone;
        }

        private static void Sync<T>(T source, T target, IMemberSettings settings)
            where T : class
        {
            ////T copy;
            ////if (TryCustomCopy(source, target, settings, out copy))
            ////{
            ////    if (copy != null && !ReferenceEquals(target, copy))
            ////    {
            ////        var message = $"The type {source.GetType()} has custom copy specified. For the root object the copy must be a side effect.\r\n" +
            ////                      $"This means that the custom copy must return null or the target instance.\r\n" +
            ////                      $"Also it makes little sense using this method with custom copy for the root type.";
            ////        throw new InvalidOperationException(message);
            ////    }

            ////    return copy;
            ////}

            using (var borrowed = settings.ReferenceHandling == ReferenceHandling.Structural
                                   ? ReferencePairCollection.Borrow()
                                   : null)
            {
                Sync(source, target, settings, borrowed?.Value);
            }
        }

        private static void Sync<T>(T source, T target, IMemberSettings settings, ReferencePairCollection referencePairs)
        {
            Debug.Assert(source != null, nameof(source));
            Debug.Assert(target != null, nameof(target));
            Debug.Assert(source.GetType() == target.GetType(), "Must be same type");
            Verify.CanCopyMemberValues(source, target, settings);

            if (referencePairs?.Add(source, target) == false)
            {
                return;
            }

            T copy;
            if (TryCustomCopy(source, target, settings, out copy))
            {
                return;
            }

            CollectionItems(source, target, settings, referencePairs);
            Members(source, target, settings, referencePairs);
        }

        private static bool TryCloneWithoutSync<T, TSettings>(
            T sourceItem,
            T targetItem,
            TSettings settings,
            out T clone)
            where TSettings : class, IMemberSettings
        {
            if (sourceItem == null ||
                settings.ReferenceHandling == ReferenceHandling.References ||
                ReferenceEquals(sourceItem, targetItem))
            {
                clone = sourceItem;
                return false;
            }

            T copy;
            if (TryCopyValue(sourceItem, targetItem, settings, out copy))
            {
                clone = copy;
                return false;
            }

            if (TryCustomCopy(sourceItem, targetItem, settings, out clone))
            {
                return false;
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    clone = sourceItem;
                    return false;
                case ReferenceHandling.Structural:
                    if (targetItem == null)
                    {
                        clone = (T)CreateInstance(sourceItem, settings);
                    }
                    else
                    {
                        clone = targetItem;
                    }

                    return true;
                case ReferenceHandling.Throw:
                    throw State.Throw.ShouldNeverGetHereException();
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(settings.ReferenceHandling),
                        settings.ReferenceHandling,
                        null);
            }
        }

        private static bool TryCopyValue<T>(T x, T y, IMemberSettings settings, out T result)
        {
            if (ReferenceEquals(x, y))
            {
                result = x;
                return true;
            }

            if (x == null)
            {
                result = x;
                return true;
            }

            if (settings.IsImmutable(x.GetType()))
            {
                result = x;
                return true;
            }

            result = default(T);
            return false;
        }

        private static bool TryCustomCopy<T>(T source, T target, IMemberSettings settings, out T copy)
        {
            CustomCopy copyer;
            if (settings.TryGetCopyer(source.GetType(), out copyer))
            {
                copy = ((CustomCopy<T>)copyer).Copy(source, target);
                return true;
            }

            copy = default(T);
            return false;
        }
    }
}
