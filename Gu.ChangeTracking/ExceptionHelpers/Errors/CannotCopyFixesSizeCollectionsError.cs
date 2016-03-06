using System.Collections;

namespace Gu.ChangeTracking
{
    using System;
    using System.Linq;
    using System.Text;

    internal sealed class CannotCopyFixesSizeCollectionsError : Error, INotSupported, IExcludableType
    {
        private readonly IEnumerable source;
        private readonly IEnumerable target;

        public CannotCopyFixesSizeCollectionsError(IEnumerable source, IEnumerable target)
        {
            this.source = source;
            this.target = target;
        }

        public Type Type => this.target.GetType();

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            errorBuilder.AppendLine($"The collections are fixed size type: {this.target.GetType().PrettyName()}")
                        .AppendLine($"  - Source count: {this.source.OfType<object>().Count()}")
                        .AppendLine($"  - Target count: {this.target.OfType<object>().Count()}");
            return errorBuilder;
        }
    }
}