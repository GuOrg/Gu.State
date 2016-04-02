namespace Gu.State
{
    internal static class Try
    {
        internal static bool CastAs<T>(object x, object y, out T xResult, out T yResult)
        {
            if (x is T && y is T)
            {
                xResult = (T)x;
                yResult = (T)y;
                return true;
            }

            xResult = default(T);
            yResult = default(T);
            return false;
        }
    }
}
