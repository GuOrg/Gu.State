namespace Gu.State
{
    using System;
    using System.Text;

    internal sealed class CannotCreateInstanceError : Error, INotSupported
    {
        private readonly object sourceValue;

        internal CannotCreateInstanceError(object sourceValue)
        {
            this.sourceValue = sourceValue;
        }

        internal Type Type => this.sourceValue.GetType();

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            errorBuilder.AppendLine($"{typeof(Activator).Name}.{nameof(Activator.CreateInstance)} failed for type {this.Type.PrettyName()}.");
            return errorBuilder;
        }
    }
}
