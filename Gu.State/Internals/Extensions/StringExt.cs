namespace Gu.State
{
    using System;
    using System.Globalization;

    internal static class StringExt
    {
        internal static string ToInvariantOrNullString(this object o)
        {
            if (o == null)
            {
                return "null";
            }

            if (o is DateTime)
            {
                return ((DateTime)o).ToString("O", CultureInfo.InvariantCulture);
            }

            if (o is bool)
            {
                return (bool)o
                           ? "true"
                           : "false";
            }

            if (o is IFormattable formattable)
            {
                return formattable.ToString($"", CultureInfo.InvariantCulture);
            }

            return o.ToString();
        }
    }
}