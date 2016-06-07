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

            var formattable = o as IFormattable;
            if (formattable != null)
            {
                return formattable.ToString($"", CultureInfo.InvariantCulture);
            }

            return o.ToString();
        }
    }
}