namespace Gu.ChangeTracking
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    [DebuggerDisplay("{GetType().Name} Indexer: {Indexer.Name}")]
    internal class UnsupportedIndexer : TypeError, IExcludable
    {
        public UnsupportedIndexer(Type type, PropertyInfo indexer)
            : base(type)
        {
            Debug.Assert(indexer.GetIndexParameters().Length > 0, "Must be an indexer");
            this.Indexer = indexer;
        }

        public PropertyInfo Indexer { get; }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine($"  - The property {this.Type.PrettyName()}.{this.Indexer.Name} is an indexer and not supported.");
        }

        StringBuilder IExcludable.AppendSuggestExclude(StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine($"    - Exclude the indexer property {this.Type.PrettyName()}.{this.Indexer.Name}.");
        }
    }
}