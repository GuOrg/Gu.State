// ReSharper disable RedundantArgumentDefaultValue
#pragma warning disable CA1825 // Avoid zero-length array allocations.
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
namespace Gu.State.Tests.EqualByTests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using NUnit.Framework;

    using static EqualByTypes;

    public static class TestCases
    {
        public static readonly TestCaseData[] WhenEqual =
        {
            new TestCaseData(null, null),
            new TestCaseData(1, 1),
            new TestCaseData((int?)null, (int?)null),
            new TestCaseData((int?)1, (int?)1),

            new TestCaseData(StringComparison.Ordinal, StringComparison.Ordinal),
            new TestCaseData((StringComparison?)null, (StringComparison?)null),
            new TestCaseData((StringComparison?)StringComparison.Ordinal, (StringComparison?)StringComparison.Ordinal),

            new TestCaseData(new Point(1, 2), new Point(1, 2)),
            new TestCaseData((Point?)new Point(1, 2), (Point?)new Point(1, 2)),

            new TestCaseData(new With<int>(1), new With<int>(1)),
            new TestCaseData(new With<int?>(1), new With<int?>(1)),
            new TestCaseData(new With<int?>(null), new With<int?>(null)),

            new TestCaseData(new With<StringComparison>(StringComparison.Ordinal), new With<StringComparison>(StringComparison.Ordinal)),
            new TestCaseData(new With<StringComparison?>(StringComparison.Ordinal), new With<StringComparison?>(StringComparison.Ordinal)),
            new TestCaseData(new With<StringComparison?>(null), new With<StringComparison?>(null)),

            new TestCaseData(new With<Guid>(Guid.Parse("f062db24-d4b8-452a-904c-ba2d23663e92")), new With<Guid>(Guid.Parse("f062db24-d4b8-452a-904c-ba2d23663e92"))),

            new TestCaseData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new WithSimpleValues(1, 2, null, StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, 2, null, StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries)),

            new TestCaseData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new WithSimpleProperties(1, 2, null, StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, 2, null, StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new WithSimpleProperties(1, null, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, null, "3", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new Derived1 { BaseValue = 1, Derived1Value = 2 }, new Derived1 { BaseValue = 1, Derived1Value = 2 }),

            new TestCaseData((int[])null, (int[])null),
            new TestCaseData(new int[0], new int[0]),
            new TestCaseData(new[] { 1, 2, 3 }, new[] { 1, 2, 3 }),

            new TestCaseData(new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } }, new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } }),

            //new TestCaseData(
            //    new With<Point>(new Point(1, 2)),
            //    new With<Point>(new Point(1, 2))),
            //new TestCaseData(
            //    new With<Point?>(new Point(1, 2)),
            //    new With<Point?>(new Point(1, 2))),
            //new TestCaseData(
            //    new With<Point?>(null),
            //    new With<Point?>(null)),
        };

        public static readonly TestCaseData[] WhenEqualStructural =
        {
            new TestCaseData(new With<int[]>(null), new With<int[]>(null)),
            new TestCaseData(new With<int[]>(new int[0]), new With<int[]>(new int[0])),
            new TestCaseData(new With<int[]>(new[] { 1, 2, 3 }), new With<int[]>(new[] { 1, 2, 3 })),

            new TestCaseData(new With<List<int>>(null), new With<List<int>>(null)),
            new TestCaseData(new With<List<int>>(new List<int>()), new With<List<int>>(new List<int>())),
            new TestCaseData(new With<List<int>>(new List<int> { 1, 2, 3 }), new With<List<int>>(new List<int> { 1, 2, 3 })),

            new TestCaseData(new With<IReadOnlyList<int>>(null), new With<IReadOnlyList<int>>(null)),
            new TestCaseData(new With<IReadOnlyList<int>>(new int[0]), new With<IReadOnlyList<int>>(new int[0])),
            new TestCaseData(new With<IReadOnlyList<int>>(new[] { 1, 2, 3 }), new With<IReadOnlyList<int>>(new[] { 1, 2, 3 })),
            new TestCaseData(new With<BaseClass>(new Derived1 { BaseValue = 1, Derived1Value = 2 }), new With<BaseClass>(new Derived1 { BaseValue = 1, Derived1Value = 2 })),
        };

        public static readonly TestCaseData[] WhenNotEqual =
        {
            new TestCaseData(null, 1),
            new TestCaseData(1, 2),
            new TestCaseData((int?)null, (int?)1),
            new TestCaseData((int?)null, (int?)1),

            new TestCaseData(StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase),
            new TestCaseData((StringComparison?)null, (StringComparison?)StringComparison.Ordinal),
            new TestCaseData((StringComparison?)StringComparison.Ordinal, (StringComparison?)StringComparison.OrdinalIgnoreCase),

            new TestCaseData(new Point(1, 2), new Point(1, -2)),
            new TestCaseData(new Point(1, 2), new Point(-1, 2)),
            new TestCaseData((Point?)new Point(1, 2), (Point?)new Point(1, -2)),
            new TestCaseData((Point?)new Point(1, 2), (Point?)new Point(-1, 2)),
            new TestCaseData((Point?)new Point(1, 2), (Point?)null),

            new TestCaseData(new With<int>(1), new With<int>(2)),
            new TestCaseData(new With<int?>(1), new With<int?>(2)),
            new TestCaseData(new With<int?>(1), new With<int?>(null)),
            new TestCaseData(new With<int>(1), (With<int>)null),

            new TestCaseData(new With<StringComparison>(StringComparison.Ordinal), new With<StringComparison>(StringComparison.OrdinalIgnoreCase)),
            new TestCaseData(new With<StringComparison?>(StringComparison.Ordinal), new With<StringComparison?>(StringComparison.OrdinalIgnoreCase)),
            new TestCaseData(new With<StringComparison?>(StringComparison.Ordinal), new With<StringComparison?>(null)),

            new TestCaseData(new With<Guid>(Guid.Parse("f062db24-d4b8-452a-904c-ba2d23663e92")), new With<Guid>(Guid.Parse("f062db24-d4b8-452a-904c-ba2d23663e93"))),

            new TestCaseData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(5, 2, "3", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, 5, "3", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, 5, null, StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, 2, "5", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, 2, "3", StringSplitOptions.None)),

            new TestCaseData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(5, 2, "3", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, 5, "3", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, null, "3", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, 5, null, StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, 2, "5", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, 2, "3", StringSplitOptions.None)),

            new TestCaseData(new Derived1 { BaseValue = 1, Derived1Value = 2 }, new Derived1 { BaseValue = -1, Derived1Value = 2 }),
            new TestCaseData(new Derived1 { BaseValue = 1, Derived1Value = 2 }, new Derived1 { BaseValue = 1, Derived1Value = -2 }),

            new TestCaseData((int[])null, new int[0]),
            new TestCaseData((int[])null, new[] { 1, 2, 3 }),
            new TestCaseData(new int[0], new[] { 1, 2, 3 }),
            new TestCaseData(new[] { 1, 2, 3 }, new[] { 1, 2 }),
            new TestCaseData(new[] { 1, 2, 3 }, new[] { 1, 2, -1 }),
            new TestCaseData(new[] { 1, 2, 3 }, new[] { -1, 2, 3 }),

            new TestCaseData(new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } }, new[,] { { -1, 2 }, { 3, 4 }, { 5, 6 } }),
            new TestCaseData(new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } }, new[,] { { 1, 2 }, { -3, 4 }, { 5, 6 } }),
            new TestCaseData(new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } }, new[,] { { 1, 2 }, { 3, 4 }, { 5, -6 } }),

            //new TestCaseData(
            //    new With<Point>(new Point(1, 2)),
            //    new With<Point>(new Point(1, 3))),
            //new TestCaseData(
            //    new With<Point>(new Point(1, 2)),
            //    new With<Point>(new Point(0, 2))),
            //new TestCaseData(
            //    new With<Point?>(new Point(1, 2)),
            //    new With<Point?>(new Point(1, 3))),
            //new TestCaseData(
            //    new With<Point?>(new Point(1, 2)),
            //    new With<Point?>(null)),
        };

        public static readonly TestCaseData[] WhenNotEqualStructural =
        {
            new TestCaseData(
                new With<int[]>(new[] { 1, 2, 3 }),
                new With<int[]>(new[] { 1, 2, 4 })),
            new TestCaseData(
                new With<int[]>(new[] { 1, 2, 3 }),
                new With<int[]>(new[] { 0, 2, 3 })),
            new TestCaseData(
                new With<int[]>(new[] { 1, 2 }),
                new With<int[]>(new[] { 1, 2, 3 })),
            new TestCaseData(
                new With<int[]>(new[] { 1, 2 }),
                new With<int[]>(null)),
            new TestCaseData(
                new With<int[]>(new int[0]),
                new With<int[]>(null)),

            new TestCaseData(
                new With<List<int>>(new List<int> { 1, 2, 3 }),
                new With<List<int>>(new List<int> { 1, 2, 4 })),
            new TestCaseData(
                new With<List<int>>(new List<int> { 1, 2, 3 }),
                new With<List<int>>(new List<int> { 0, 2, 3 })),
            new TestCaseData(
                new With<List<int>>(new List<int> { 1, 2 }),
                new With<List<int>>(new List<int> { 1, 2, 3 })),
            new TestCaseData(
                new With<List<int>>(new List<int> { 1, 2 }),
                new With<List<int>>(null)),
            new TestCaseData(
                new With<List<int>>(new List<int>()),
                new With<List<int>>(null)),

            new TestCaseData(
                new With<IReadOnlyList<int>>(new[] { 1, 2, 3 }),
                new With<IReadOnlyList<int>>(new[] { 1, 2, 4 })),
            new TestCaseData(
                new With<IReadOnlyList<int>>(new[] { 1, 2, 3 }),
                new With<IReadOnlyList<int>>(new[] { 0, 2, 3 })),
            new TestCaseData(
                new With<IReadOnlyList<int>>(new[] { 1, 2 }),
                new With<IReadOnlyList<int>>(new[] { 1, 2, 3 })),
            new TestCaseData(
                new With<IReadOnlyList<int>>(new[] { 1, 2 }),
                new With<IReadOnlyList<int>>(null)),
            new TestCaseData(
                new With<IReadOnlyList<int>>(new int[0]),
                new With<IReadOnlyList<int>>(null)),

            new TestCaseData(
                new With<BaseClass>(new Derived1 { BaseValue = 1, Derived1Value = 2 }),
                new With<BaseClass>(new Derived1 { BaseValue = 1, Derived1Value = -2 })),
            new TestCaseData(
                new With<BaseClass>(new Derived1()),
                new With<BaseClass>(new Derived2())),

        };
    }
}