namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    public abstract class Diff
    {
        private static readonly IReadOnlyList<SubDiff> Empty = new SubDiff[0];

        internal Diff(IReadOnlyList<SubDiff> diffs = null)
        {
            this.Diffs = diffs ?? Empty;
        }

        public IReadOnlyList<SubDiff> Diffs { get; }

        public abstract string ToString(string tabString, string newLine);

        internal abstract IndentedTextWriter WriteDiffs(IndentedTextWriter writer);
    }
}
