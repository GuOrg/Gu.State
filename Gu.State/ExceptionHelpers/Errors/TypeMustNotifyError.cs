namespace Gu.State
{
    using System;
    using System.ComponentModel;
    using System.Text;

    internal sealed class TypeMustNotifyError : TypeError, IFixWithImmutable, IExcludableType, INotSupported, IFixWithNotify
    {
        public TypeMustNotifyError(Type type)
            : base(type)
        {
        }

        public static bool operator ==(TypeMustNotifyError left, TypeMustNotifyError right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TypeMustNotifyError left, TypeMustNotifyError right)
        {
            return !Equals(left, right);
        }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine($"The type {this.Type.PrettyName()} does not notify changes.");
        }

        public StringBuilder AppendSuggestFixWithNotify(StringBuilder errorBuilder)
        {
            var propChanged = typeof(INotifyPropertyChanged).Name;
            var line = this.Type.Assembly == typeof(string).Assembly
                           ? $"* Use a type that implements {propChanged} isntead of {this.Type.PrettyName()}."
                           : $"* Implement {propChanged} for {this.Type.PrettyName()} or use a type that does.";
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

            return obj is TypeMustNotifyError && this.Equals((TypeMustNotifyError)obj);
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }

        private bool Equals(TypeMustNotifyError other)
        {
            return this.Type == other.Type;
        }
    }
}