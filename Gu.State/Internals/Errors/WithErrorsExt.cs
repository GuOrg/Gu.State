namespace Gu.State
{
    using System.Linq;

    internal static class WithErrorsExt
    {
        internal static bool HasError<T>(this IWithErrors withErrors)
        {
            return withErrors.Errors.Any(x => x is T);
        }
    }
}
