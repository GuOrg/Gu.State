namespace Gu.ChangeTracking
{
    using System.Text;

    internal interface IExcludableMember
    {
        StringBuilder AppendSuggestExclude(StringBuilder errorBuilder);
    }
}