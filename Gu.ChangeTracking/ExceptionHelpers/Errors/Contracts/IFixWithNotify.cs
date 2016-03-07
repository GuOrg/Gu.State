namespace Gu.ChangeTracking
{
    using System.Text;

    internal interface IFixWithNotify
    {
        StringBuilder AppendSuggestFixWithNotify(StringBuilder errorBuilder);
    }
}