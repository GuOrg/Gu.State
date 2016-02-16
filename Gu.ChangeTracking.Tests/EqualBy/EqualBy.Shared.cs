namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Collections.Generic;

    using Gu.ChangeTracking.Tests.CopyStubs;

    public partial class EqualByTests
    {
        public static IReadOnlyList<EqualsData> EqualsSource = new List<EqualsData>
        {
            new EqualsData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           true),
            new EqualsData(new WithSimpleProperties(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleProperties(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           true),
            new EqualsData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleProperties(5, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleProperties(1, 5, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleProperties(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleProperties(1, 2, "5", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleProperties(1, 2, "3", StringSplitOptions.None),
                           false),
        };

        public class EqualsData
        {
            public EqualsData(object source, object target, bool @equals)
            {
                this.Source = source;
                this.Target = target;
                this.Equals = @equals;
            }

            public object Source { get; }

            public object Target { get; }

            public bool Equals { get; }
        }
    }
}
