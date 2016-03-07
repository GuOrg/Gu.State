namespace Gu.State
{
    using System.Text;

    internal interface IFixWithNotify
    {
        StringBuilder AppendSuggestFixWithNotify(StringBuilder errorBuilder);
    }
}