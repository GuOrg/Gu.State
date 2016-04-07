namespace Gu.State
{
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{GetType().Name} Loop: {Path.PathString}")]
    internal sealed class CollectionErrors : Error, IWithErrors
    {
        public CollectionErrors(MemberPath memberPath, TypeErrors typeErrors)
        {
            this.Path = memberPath;
            this.Errors = new[] { typeErrors };
        }

        public MemberPath Path { get; }

        public IReadOnlyList<Error> Errors { get; }
    }
}