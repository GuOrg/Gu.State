namespace Gu.State
{
    using System.Collections.Generic;

    internal interface IWithErrors
    {
        IReadOnlyCollection<Error> Errors { get; }
    }
}