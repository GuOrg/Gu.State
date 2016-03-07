namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Text;

    [DebuggerDisplay("{GetType().Name} Indexer: {Type.Name}")]
    internal sealed class NotCopyableCollection : TypeError, INotSupported, IExcludableType
    {
        public NotCopyableCollection(Type type)
            : base(type)
        {
        }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine($"  - The type {this.Type.PrettyName()} is a collection but does not implement IList or IDictionary and is not supported.");
        }
    }
}