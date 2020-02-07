namespace Gu.State.Tests
{
    using System.Collections;

    using NUnit.Framework;

    public class ResetEventArgsComparer : EventArgsComparer<ResetEventArgs>
    {
        public static readonly ResetEventArgsComparer Default = new ResetEventArgsComparer();

        public override bool Equals(ResetEventArgs expected, ResetEventArgs actual)
        {
            if (!ReferenceEquals(expected.Source, actual.Source))
            {
                throw new AssertionException($"Expected source to be same.");
            }

            return true;
        }

        private static bool Equals(IList x, IList y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            if (x.Count != y.Count)
            {
                return false;
            }

            for (int i = 0; i < x.Count; i++)
            {
                if (!Equals(x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}