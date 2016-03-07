namespace Gu.ChangeTracking
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    public static partial class Ensure
    {
        // ReSharper disable once UnusedParameter.Global
        internal static void NotNullOrEmpty(string value, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        // ReSharper disable once UnusedParameter.Global
        public static void IsMatch(string text, string pattern, string parameterName)
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            if (!Regex.IsMatch(text, pattern))
            {
                throw new ArgumentException(parameterName);
            }
        }
    }
}
