namespace Gu.ChangeTracking
{
    using System;

    internal static class Throw
    {
        internal static readonly string ThereIsABugInTheLibrary = "There is a bug in the library as it:";

        internal static InvalidOperationException ExpectedParameterOfTypes<T1, T2>(string parameterName)
        {
            var message = $"Expected {nameof(parameterName)} to be either of {typeof(T1).PrettyName()} or {typeof(T2).PrettyName()}";
            return ThrowThereIsABugInTheLibrary(message);
        }

        internal static InvalidOperationException ShouldNeverGetHereException()
        {
            var message = "Should never gete here";
            return ThrowThereIsABugInTheLibrary(message);
        }

        internal static Exception ShouldNeverGetHereException(string message)
        {
            var text = "Should never gete here\r\n" + message;
            return ThrowThereIsABugInTheLibrary(text);
        }

        internal static InvalidOperationException ThrowThereIsABugInTheLibrary(string message)
        {
            message = $"{ThereIsABugInTheLibrary}\r\n" + message;
            return new InvalidOperationException(message);
        }
    }
}