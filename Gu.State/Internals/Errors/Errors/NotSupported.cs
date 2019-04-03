namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Text;

    [DebuggerDisplay("{GetType().Name} Type: {Type.Name}")]
    internal sealed class NotSupported : Error, INotSupported
    {
        public NotSupported(Type type, string message)
        {
            this.Type = type;
            this.Message = message;
        }

        public Type Type { get; }

        public string Message { get; }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            if (this.Message is string text)
            {
                return errorBuilder.AppendLine($"  - {text}");
            }

            return errorBuilder.AppendLine($"  - The type {this.Type.PrettyName()} is not supported.");
        }
    }
}