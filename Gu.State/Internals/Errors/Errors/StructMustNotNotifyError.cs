namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Text;

    internal sealed class StructMustNotNotifyError : Error, IFixWithImmutable, INotSupported
    {
        private static readonly ConcurrentDictionary<Type, StructMustNotNotifyError> Cache = new ConcurrentDictionary<Type, StructMustNotNotifyError>();

        private StructMustNotNotifyError(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            var notifier = this.Type.Implements<INotifyPropertyChanged>() ? "INotifyPropertyChanged" : "INotifyCollectionChanged";
            return errorBuilder.AppendLine($"The type {this.Type.PrettyName()} is a mutable struct that implements {notifier}.")
                               .AppendLine("  As it is a value type subscribing to changes does not make sense.");
        }

        internal static StructMustNotNotifyError GetOrCreate(Type type)
        {
            Debug.Assert(type.IsValueType, "Must be a struct");
            Debug.Assert(type.Implements<INotifyPropertyChanged>() || type.Implements<INotifyCollectionChanged>(), "Must notify");
            return Cache.GetOrAdd(type, t => new StructMustNotNotifyError(t));
        }
    }
}