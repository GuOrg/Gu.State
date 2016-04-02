namespace Gu.State
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    public abstract class Diff
    {
        private static readonly IReadOnlyCollection<Diff> Empty = new Diff[0];

        internal Diff(IReadOnlyCollection<Diff> diffs = null)
        {
            this.Diffs = diffs ?? Empty;
        }

        public IReadOnlyCollection<Diff> Diffs { get; }

        public abstract string ToString(string tabString, string newLine);

        internal abstract IndentedTextWriter WriteDiffs(IndentedTextWriter writer);
    }
}
