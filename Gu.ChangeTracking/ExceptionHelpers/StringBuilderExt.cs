namespace Gu.ChangeTracking
{
    using System.Text;

    internal static partial class StringBuilderExt
    {
        internal static StringBuilder AppendSolveTheProblemBy(this StringBuilder errorBuilder)
        {
            return errorBuilder.AppendLine("Solve the problem by any of:");
        }
    }
}
