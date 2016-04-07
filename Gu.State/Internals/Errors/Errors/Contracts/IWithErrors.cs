namespace Gu.State
{
    using System.Collections.Generic;

    internal interface IWithErrors
    {
        IReadOnlyList<Error> Errors { get; }
    }
}