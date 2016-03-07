namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text;

    internal sealed class CollectionMustNotifyError : TypeError, IFixWithImmutable, IExcludableType, INotSupported, IFixWithNotify
    {
        public CollectionMustNotifyError(Type type)
            : base(type)
        {
        }

        public static bool operator ==(CollectionMustNotifyError left, CollectionMustNotifyError right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CollectionMustNotifyError left, CollectionMustNotifyError right)
        {
            return !Equals(left, right);
        }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine($"The collection type {this.Type.PrettyName()} does not notify changes.");
        }

        public StringBuilder AppendSuggestFixWithNotify(StringBuilder errorBuilder)
        {
            var colChanged = typeof(INotifyCollectionChanged).Name;
            var line = this.Type.Assembly == typeof(List<>).Assembly
                           ? $"* Use a type that implements {colChanged} instead of {this.Type.PrettyName()}."
                           : $"* Implement {colChanged} for {this.Type.PrettyName()} or use a type that does.";
            return errorBuilder.AppendLine(line);
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

            return obj is CollectionMustNotifyError && this.Equals((CollectionMustNotifyError)obj);
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }

        private bool Equals(CollectionMustNotifyError other)
        {
            return this.Type == other.Type;
        }
    }
}