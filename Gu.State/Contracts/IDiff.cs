namespace Gu.State
{
    using System.Collections.Generic;

    public interface IDiff
    {
        IReadOnlyCollection<IDiff> Diffs { get; }

        bool IsEmpty { get; }
    }
}
