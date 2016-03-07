namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text;

    internal sealed class CollectionMustNotifyError : TypeError, IFixWithImmutable, IExcludableType, INotSupported
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
            var line = this.Type.Assembly == typeof(List<>).Assembly
                           ? $"The collection type {this.Type.PrettyName()} does not notify changes. Use a type that implements {typeof(INotifyCollectionChanged).Name}."
                           : $"The collection type {this.Type.PrettyName()} does not notify changes. Implement {typeof(INotifyCollectionChanged).Name} or use a type that implements {typeof(INotifyCollectionChanged).Name}.";
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