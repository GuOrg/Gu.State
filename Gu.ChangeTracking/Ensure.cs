namespace Gu.ChangeTracking
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal static class Ensure
    {
        internal static void NotNull(object o, string parameterName, [CallerMemberName] string caller = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName));
            if (o == null)
            {
                var message = $"Expected parameter {parameterName} in member {caller} to not be null";
                throw new ArgumentNullException(parameterName, message);
            }
        }

        internal static void NotNull(object o, string parameterName, string message, [CallerMemberName] string caller = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName));
            if (o == null)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(parameterName);
                }
                throw new ArgumentNullException(parameterName, message);
            }
        }

        internal static void NotNullOrEmpty(string s, string paramName, string message = null)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (message == null)
                {
                    throw new ArgumentNullException(paramName);
                }
                throw new ArgumentNullException(paramName, message);
            }
        }

        public static void NotEqual<T>(T value, T other, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName));
            if (Equals(value, other))
            {
                var message = $"Expected {value} to not equal {other}";
                throw new ArgumentException(message, parameterName);
            }
        }
    }
}
