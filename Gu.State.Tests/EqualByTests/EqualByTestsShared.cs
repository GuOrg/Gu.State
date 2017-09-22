namespace Gu.State.Tests.EqualByTests
{
    using System;
    using System.Collections.Generic;

    public class EqualByTestsShared
    {
        public static IReadOnlyList<EqualsData> EqualsSource = new List<EqualsData>
        {
            new EqualsData(
                source: new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                target: new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                @equals: true),
            new EqualsData(
                source: new EqualByTypes.WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                target: new EqualByTypes.WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                @equals: true),
            new EqualsData(
                source: new EqualByTypes.With<IReadOnlyList<int>>(new[]{ 1, 2, 3 }),
                target: new EqualByTypes.With<IReadOnlyList<int>>(new[]{ 1, 2, 3 }),
                @equals: true),
            new EqualsData(
                source: new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                target: new EqualByTypes.WithSimpleValues(5, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                @equals: false),
            new EqualsData(
                source: new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                target: new EqualByTypes.WithSimpleValues(1, 5, "3", StringSplitOptions.RemoveEmptyEntries),
                @equals: false),
            new EqualsData(
                source: new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                target: new EqualByTypes.WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                @equals: false),
            new EqualsData(
                source: new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                target: new EqualByTypes.WithSimpleValues(1, 2, "5", StringSplitOptions.RemoveEmptyEntries),
                @equals: false),
            new EqualsData(
                source: new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                target: new EqualByTypes.WithSimpleValues(1, 2, "3", StringSplitOptions.None),
                @equals: false),
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
                return $"Type: {this.Source?.GetType().PrettyName() ?? "null"} Source: {this.Source}, Target: {this.Target}, Equals: {this.Equals}";
            }
        }
    }
}
