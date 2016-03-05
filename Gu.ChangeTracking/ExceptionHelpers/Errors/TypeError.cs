namespace Gu.ChangeTracking
{
    using System;
    using System.Text;

    internal abstract class TypeError : Error, IFixWithEquatable, IExcludable
    {
        public TypeError(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; }

        StringBuilder IExcludable.AppendSuggestExclude(StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine($"    - Exclude the type {this.Type.PrettyName()}.");
        }
    }
}