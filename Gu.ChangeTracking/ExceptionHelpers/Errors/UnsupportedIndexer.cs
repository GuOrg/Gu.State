namespace Gu.ChangeTracking
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    [DebuggerDisplay("{GetType().Name} Indexer: {Indexer.Name}")]
    internal class UnsupportedIndexer : TypeError
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
            return errorBuilder.AppendLine($"The property {this.Type.PrettyName()}.{this.Indexer.Name} of type {this.Indexer.PropertyType.PrettyName()} is an indexer and not supported.");
        }

        public StringBuilder AppendSuggestExclude(StringBuilder errorBuilder, IMemberSettings settings)
        {
            if (settings is PropertiesSettings)
            {
                return errorBuilder.AppendLine($"  - Exclude the property {this.Type.PrettyName()}.{this.Indexer.Name}.");
            }

            return errorBuilder;
        }
    }
}