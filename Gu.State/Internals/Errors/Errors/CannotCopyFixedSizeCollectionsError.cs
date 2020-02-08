namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;

    internal sealed class CannotCopyFixedSizeCollectionsError : Error, INotSupported
    {
        private readonly IEnumerable source;
        private readonly IEnumerable target;

        internal CannotCopyFixedSizeCollectionsError(IEnumerable source, IEnumerable target)
        {
            this.source = source;
            this.target = target;
        }

        internal Type Type => this.target.GetType();

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            errorBuilder.AppendLine($"The collections are fixed size type: {this.Type.PrettyName()}")
                        .AppendLine($"  - Source count: {this.source.OfType<object>().Count()}")
                        .AppendLine($"  - Target count: {this.target.OfType<object>().Count()}");
            return errorBuilder;
        }
    }
}
