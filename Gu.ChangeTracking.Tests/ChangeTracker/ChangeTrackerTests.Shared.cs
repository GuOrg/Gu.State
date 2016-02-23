namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Collections.Generic;

    public partial class ChangeTrackerTests
    {
        private static IReadOnlyList<object> CreateExpectedChangeArgs(int n)
        {
            var objects = new List<object>();
            for (int i = 0; i < n; i++)
            {
                objects.Add("Changes");
                objects.Add(EventArgs.Empty);
            }

            return objects;
        }
    }
}