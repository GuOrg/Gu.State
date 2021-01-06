#pragma warning disable SA1600 // Elements should be documented
namespace Gu.State
{
    using System.Collections.Generic;

    internal interface IWithErrors
    {
        IReadOnlyList<Error> Errors { get; }
    }
}
