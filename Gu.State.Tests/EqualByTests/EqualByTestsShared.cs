namespace Gu.State.Tests.EqualByTests
{
    using System;
    using System.Collections.Generic;

    public class EqualByTestsShared
    {
        public static IReadOnlyList<EqualsData> EqualsSource = new List<EqualsData>
        {
            new EqualsData(new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           true),
            new EqualsData(new EqualByTypes.WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           new EqualByTypes.WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           true),
            new EqualsData(new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new EqualByTypes.WithSimpleValues(5, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new EqualByTypes.WithSimpleValues(1, 5, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new EqualByTypes.WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new EqualByTypes.WithSimpleValues(1, 2, "5", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.None),
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

            public new bool Equals { get; }

            public override string ToString()
            {
                return $"Source: {this.Source}, Target: {this.Target}, Equals: {this.Equals}";
            }
        }
    }
}
