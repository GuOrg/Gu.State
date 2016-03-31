namespace Gu.State
{
    public static partial class DiffBy
    {
        private static bool TryGetValueDiff<TSettings>(object x, object y, TSettings settings, out ValueDiff diff)
            where TSettings : IMemberSettings
        {
            if (ReferenceEquals(x, y))
            {
                diff = null;
                return true;
            }

            if (x == null || y == null)
            {
                diff = new ValueDiff(x, y);
                return true;
            }

            if (x.GetType() != y.GetType())
            {
                diff = new ValueDiff(x, y);
                return true;
            }

            if (settings.IsEquatable(x.GetType()))
            {
                diff = Equals(x, y)
                           ? null
                           : new ValueDiff(x, y);
                return true;
            }

            diff = null;
            return false;
        }
    }
}
