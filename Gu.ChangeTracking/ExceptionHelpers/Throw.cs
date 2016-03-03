namespace Gu.ChangeTracking
{
    using System;

    internal static class Throw
    {
        internal static InvalidOperationException ThrowThereIsABugInTheLibraryExpectedParameterOfTypes<T1, T2>(string parameterName)
        {
            var message = $"Expected {nameof(parameterName)} to be either of {typeof(T1).PrettyName()} or {typeof(T2).PrettyName()}";
            return ThrowThereIsABugInTheLibrary(message);
        }

        internal static InvalidOperationException ThrowThereIsABugInTheLibrary(string message)
        {
            message = $"{StringBuilderExt.ThereIsABugInTheLibrary}\r\n" + message;
            return new InvalidOperationException(message);
        }
    }
}