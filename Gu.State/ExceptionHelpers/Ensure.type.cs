namespace Gu.State
{
    using System;
    using System.Diagnostics;

    public static partial class Ensure
    {
        internal static void SameType(object x, object y, string xName = null, string yName = null)
        {
            if (x?.GetType() == y?.GetType())
            {
                return;
            }

            var message = $"Expected same type, was {x?.GetType()} and {y?.GetType()}";
            if (xName != null && yName != null)
            {
                throw new ArgumentException(message, $"{xName}, {yName}");
            }

            throw new ArgumentException(message, $"{xName}, {yName}");
        }

        internal static void Is<T>(object x, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            if (x is T)
            {
                return;
            }

            throw new ArgumentException($"Expected {parameterName} to be of type {typeof(T)}", parameterName);
        }

        internal static void NotIs<T>(object x, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            if (!(x is T))
            {
                return;
            }

            throw new ArgumentException($"Expected {parameterName} to not be of type {typeof(T)}", parameterName);
        }

        internal static void IsAssignableFrom<T>(object x, string parameterName)
        {
            IsAssignableFrom(x, typeof(T), parameterName);
        }

        internal static void IsAssignableFrom(object x, Type type, string parameterName)
        {
            var xType = x as Type;
            if (xType != null)
            {
                if (!type.IsAssignableFrom(xType))
                {
                    throw new ArgumentException($"Expected {parameterName} to be {type}", parameterName);
                }

                return;
            }

            if (!type.IsInstanceOfType(x))
            {
                throw new ArgumentException($"Expected {parameterName} to be {type}", parameterName);
            }
        }

        internal static void NotAssignableFrom<T>(object x, string parameterName)
        {
            NotAssignableFrom(x, typeof(T), parameterName);
        }

        internal static void NotAssignableFrom(object x, Type type, string parameterName)
        {
            if (type.IsInstanceOfType(x))
            {
                throw new ArgumentException($"Expected {parameterName} to be {type}", parameterName);
            }
        }
    }
}
