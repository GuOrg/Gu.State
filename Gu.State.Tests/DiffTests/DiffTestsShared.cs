namespace Gu.State.Tests.DiffTests
{
    using System;
    using System.Collections.Generic;

    using static DiffTypes;

    public class DiffTestsShared
    {
        public static IReadOnlyList<DiffData> DiffSource = new List<DiffData>
        {
            new DiffData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           true),
            new DiffData(new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           true),
            new DiffData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleValues(5, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new DiffData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleValues(1, 5, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new DiffData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new DiffData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleValues(1, 2, "5", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new DiffData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleValues(1, 2, "3", StringSplitOptions.None),
                           false),
        };

        public class DiffData
        {
            public DiffData(object source, object target, bool @equals)
            {
                this.Source = source;
                this.Target = target;
                this.Equals = @equals;
            }

            public object Source { get; }

            public object Target { get; }

            public bool Equals { get; }

            public override string ToString()
            {
                return $"Source: {this.Source}, Target: {this.Target}, Equals: {this.Equals}";
            }
        }
    }
}
