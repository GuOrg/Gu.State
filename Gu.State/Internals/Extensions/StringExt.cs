namespace Gu.State
{
    using System;
    using System.Globalization;

    internal static class StringExt
    {
        internal static string ToInvariantOrNullString(this object o)
        {
            return o switch
            {
                null => "null",
                DateTime time => time.ToString("O", CultureInfo.InvariantCulture),
                bool b => b ? "true" : "false",
                IFormattable f => f.ToString($"", CultureInfo.InvariantCulture),
                _ => o.ToString(),
            };
        }
    }
}
