namespace Gu.ChangeTracking
{
    using System.Text;

    internal interface INotSupported
    {
        StringBuilder AppendNotSupported(StringBuilder errorBuilder);
    }
}