#pragma warning disable SA1600 // Elements should be documented
namespace Gu.State
{
    using System.Text;

    internal interface INotSupported
    {
        StringBuilder AppendNotSupported(StringBuilder errorBuilder);
    }
}
