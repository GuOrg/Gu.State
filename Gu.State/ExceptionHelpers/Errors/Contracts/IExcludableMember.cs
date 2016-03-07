namespace Gu.State
{
    using System.Text;

    internal interface IExcludableMember
    {
        StringBuilder AppendSuggestExcludeMember(StringBuilder errorBuilder);
    }
}