namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Text;

    [DebuggerDisplay("{GetType().Name} Type: {Type.Name}")]
    internal sealed class NotCopyableCollection : Error, INotSupported
    {
        public NotCopyableCollection(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine($"  - The type {this.Type.PrettyName()} is a collection but does not implement IList or IDictionary and is hence not supported.");
        }
    }
}