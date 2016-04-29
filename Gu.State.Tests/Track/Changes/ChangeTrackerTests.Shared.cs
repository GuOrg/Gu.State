namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;

    public partial class ChangeTrackerTests
    {
        private static IReadOnlyList<object> CreateExpectedChangeArgs(int n)
        {
            var objects = new List<object>();
            for (var i = 0; i < n; i++)
            {
                objects.Add(EventArgs.Empty);
                objects.Add("Changes");
            }

            return objects;
        }
    }
}