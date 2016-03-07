namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    [DebuggerDisplay("{GetType().Name} Indexer: {Indexer.Name}")]
    internal sealed class UnsupportedIndexer : TypeError, INotSupported, IExcludableType, IExcludableMember, IFixWithEquatable, IFixWithImmutable
    {
        public UnsupportedIndexer(Type type, PropertyInfo indexer)
            : base(type)
        {
            Debug.Assert(indexer.GetIndexParameters().Length > 0, "Must be an indexer");
            this.Indexer = indexer;
        }

        public PropertyInfo Indexer { get; }

        public static bool operator ==(UnsupportedIndexer left, UnsupportedIndexer right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UnsupportedIndexer left, UnsupportedIndexer right)
        {
            return !Equals(left, right);
        }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine($"  - The property {this.Type.PrettyName()}.{this.Indexer.Name} is an indexer and not supported.");
        }

        StringBuilder IExcludableMember.AppendSuggestExcludeMember(StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine($"    - The indexer property {this.Type.PrettyName()}.{this.Indexer.Name}.");
        }

        private bool Equals(UnsupportedIndexer other)
        {
            return this.Indexer.Equals(other.Indexer);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is UnsupportedIndexer && this.Equals((UnsupportedIndexer)obj);
        }

        public override int GetHashCode()
        {
            return this.Indexer.GetHashCode();
        }
    }
}