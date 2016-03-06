namespace Gu.ChangeTracking
{
    using System.Text;

    internal interface IExcludableMember
    {
        StringBuilder AppendSuggestExcludeMember(StringBuilder errorBuilder);
    }
}