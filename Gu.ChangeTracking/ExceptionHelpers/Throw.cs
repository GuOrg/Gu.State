namespace Gu.ChangeTracking
{
    using System;

    internal static class Throw
    {
        internal static void ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<T1, T2>(string parameterName)
        {
            var message = $"Expected {nameof(parameterName)} to be either of {typeof(T1).PrettyName()} or {typeof(T2).PrettyName()}";
            ThrowThereIsABugInTheLibrary(message);
        }

        internal static void ThrowThereIsABugInTheLibrary(string message)
        {
            message = $"{ErrorBuilder.ThereIsABugInTheLibrary}\r\n" + message;
            throw new InvalidOperationException(message);
        }
    }
}