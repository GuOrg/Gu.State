namespace Gu.State
{
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{GetType().Name} Loop: {Path.PathString}")]
    internal sealed class CollectionErrors : Error, IWithErrors
    {
        internal CollectionErrors(MemberPath memberPath, TypeErrors typeErrors)
        {
            this.Path = memberPath;
            this.Errors = new[] { typeErrors };
        }

        public IReadOnlyList<Error> Errors { get; }

        internal MemberPath Path { get; }
    }
}
