#pragma warning disable SA1600 // Elements should be documented
namespace Gu.State
{
    using System;
    using System.Text;

    internal interface IFixWithNotify
    {
        StringBuilder AppendSuggestFixWithNotify(StringBuilder errorBuilder, Type type);
    }
}
