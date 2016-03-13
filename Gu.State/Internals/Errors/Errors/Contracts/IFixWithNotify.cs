namespace Gu.State
{
    using System;
    using System.Text;

    internal interface IFixWithNotify
    {
        StringBuilder AppendSuggestFixWithNotify(StringBuilder errorBuilder, Type type);
    }
}