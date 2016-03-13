namespace Gu.State
{
    using System.Text;

    internal interface INotSupported
    {
        StringBuilder AppendNotSupported(StringBuilder errorBuilder);
    }
}