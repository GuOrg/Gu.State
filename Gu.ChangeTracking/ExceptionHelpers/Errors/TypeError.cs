namespace Gu.ChangeTracking
{
    using System;
    using System.Text;

    internal abstract class TypeError : Error
    {
        public TypeError(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; }

        public StringBuilder AppendSuggestExclude(StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine($"  - Exclude the type {this.Type.PrettyName()}.");
        }
    }
}