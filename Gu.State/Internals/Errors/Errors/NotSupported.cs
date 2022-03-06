namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Text;

    [DebuggerDisplay("{GetType().Name} Type: {Type.Name}")]
    internal sealed class NotSupported : Error, INotSupported
    {
        internal NotSupported(Type type, string message)
        {
            this.Type = type;
            this.Message = message;
        }

        internal Type Type { get; }

        internal string Message { get; }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            if (this.Message is { } text)
            {
                return errorBuilder.AppendLine($"  - {text}");
            }

            return errorBuilder.AppendLine($"  - The type {this.Type.PrettyName()} is not supported.");
        }
    }
}
