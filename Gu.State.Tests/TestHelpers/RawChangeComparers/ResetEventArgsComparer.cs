namespace Gu.State.Tests
{
    using System.Collections;

    public class ResetEventArgsComparer : EventArgsComparer<ResetEventArgs>
    {
        public static readonly ResetEventArgsComparer Default = new ResetEventArgsComparer();

        public override bool Equals(ResetEventArgs expected, ResetEventArgs actual)
        {
            return Equals(expected.OldItems, actual.OldItems) && Equals(expected.NewItems, actual.NewItems);
        }

        private static bool Equals(IList x, IList y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
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