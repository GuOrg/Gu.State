namespace Gu.ChangeTracking
{
    using System.Text;

    internal interface IExcludable
    {
        StringBuilder AppendSuggestExclude(StringBuilder errorBuilder);
    }
}