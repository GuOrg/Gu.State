namespace Gu.State
{
    using System;

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

        internal static T Item<T, TSettings>(
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

            T copy;
            if (TryCopyValue(sourceItem, targetItem, settings, out copy))
            {
                return copy;
            }

            if (TryCustomCopy(sourceItem, targetItem, settings, out copy))
            {
                return copy;
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    return sourceItem;
                case ReferenceHandling.Structural:
                    if (targetItem == null)
                    {
                        targetItem = (T)CreateInstance(sourceItem, settings);
                    }

                    Sync(sourceItem, targetItem, settings, referencePairs);
                    return targetItem;
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
