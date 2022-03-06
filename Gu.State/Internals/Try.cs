namespace Gu.State
{
    internal static class Try
    {
        internal static bool CastAs<T>(object x, object y, out T xResult, out T yResult)
        {
            if (x is T xt &&
                y is T yt)
            {
                xResult = xt;
                yResult = yt;
                return true;
            }

            xResult = default;
            yResult = default;
            return false;
        }
    }
}
