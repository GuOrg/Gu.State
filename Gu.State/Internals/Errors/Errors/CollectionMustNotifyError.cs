namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Text;

    internal sealed class CollectionMustNotifyError : Error, IFixWithImmutable, INotSupported, IFixWithNotify
    {
        private static readonly ConcurrentDictionary<Type, CollectionMustNotifyError> Cache = new();

        private CollectionMustNotifyError(Type type)
        {
            this.Type = type;
        }

        internal Type Type { get; }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine($"The collection type {this.Type.PrettyName()} does not notify changes.");
        }

        public StringBuilder AppendSuggestFixWithNotify(StringBuilder errorBuilder, Type type)
        {
            var colChanged = typeof(INotifyCollectionChanged).Name;
            var line = type.Assembly == typeof(List<>).Assembly
                           ? $"* Use a type that implements {colChanged} instead of {type.PrettyName()}."
                           : $"* Implement {colChanged} for {type.PrettyName()} or use a type that does.";
            return errorBuilder.AppendLine(line);
        }

        internal static CollectionMustNotifyError GetOrCreate(Type type)
        {
            Debug.Assert(typeof(IEnumerable).IsAssignableFrom(type), "Must be an enumerable");
            return Cache.GetOrAdd(type, t => new CollectionMustNotifyError(t));
        }
    }
}
