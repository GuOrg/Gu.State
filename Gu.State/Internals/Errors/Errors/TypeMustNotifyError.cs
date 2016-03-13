namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Text;

    internal sealed class TypeMustNotifyError : Error, IFixWithImmutable, INotSupported, IFixWithNotify
    {
        private static readonly ConcurrentDictionary<Type, TypeMustNotifyError> Cache = new ConcurrentDictionary<Type, TypeMustNotifyError>();

        private TypeMustNotifyError(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine($"The type {this.Type.PrettyName()} does not notify changes.");
        }

        public StringBuilder AppendSuggestFixWithNotify(StringBuilder errorBuilder, Type type)
        {
            var propChanged = typeof(INotifyPropertyChanged).Name;
            var line = type.Assembly == typeof(string).Assembly
                           ? $"* Use a type that implements {propChanged} isntead of {type.PrettyName()}."
                           : $"* Implement {propChanged} for {type.PrettyName()} or use a type that does.";
            return errorBuilder.AppendLine(line);
        }

        internal static TypeMustNotifyError GetOrCreate(Type type)
        {
            return Cache.GetOrAdd(type, t => new TypeMustNotifyError(t));
        }
    }
}