// ReSharper disable RedundantArgumentDefaultValue
#pragma warning disable CA1825 // Avoid zero-length array allocations.
namespace Gu.State.Tests.EqualByTests
{
    using System;
    using NUnit.Framework;

    using static EqualByTypes;

    public static class TestCases
    {
        public static readonly TestCaseData[] WhenEqual =
        {
            new TestCaseData(
                new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(
                new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries)),

            //new TestCaseData(
            //    new With<int[]>(null),
            //    new With<int[]>(null)),
            //new TestCaseData(
            //    new With<int[]>(new int[0]),
            //    new With<int[]>(new int[0])),
            //new TestCaseData(
            //    new With<int[]>(new[] { 1, 2, 3 }),
            //    new With<int[]>(new[] { 1, 2, 3 })),

            //new TestCaseData(
            //    new With<List<int>>(null),
            //    new With<List<int>>(null)),
            //new TestCaseData(
            //    new With<List<int>>(new List<int>()),
            //    new With<List<int>>(new List<int>())),
            //new TestCaseData(
            //    new With<List<int>>(new List<int> { 1, 2, 3 }),
            //    new With<List<int>>(new List<int> { 1, 2, 3 })),

            //new TestCaseData(
            //    new With<IReadOnlyList<int>>(null),
            //    new With<IReadOnlyList<int>>(null)),
            //new TestCaseData(
            //    new With<IReadOnlyList<int>>(new int[0]),
            //    new With<IReadOnlyList<int>>(new int[0])),
            //new TestCaseData(
            //    new With<IReadOnlyList<int>>(new[] { 1, 2, 3 }),
            //    new With<IReadOnlyList<int>>(new[] { 1, 2, 3 })),

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

        public static readonly TestCaseData[] WhenNotEqual =
        {
            new TestCaseData(
                new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                new WithSimpleValues(5, 2, "3", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(
                new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                new WithSimpleValues(1, 5, "3", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(
                new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(
                new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                new WithSimpleValues(1, 2, "5", StringSplitOptions.RemoveEmptyEntries)),
            new TestCaseData(
                new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                new WithSimpleValues(1, 2, "3", StringSplitOptions.None)),

            //new TestCaseData(
            //    new With<int[]>(new[] { 1, 2, 3 }),
            //    new With<int[]>(new[] { 1, 2, 4 })),
            //new TestCaseData(
            //    new With<int[]>(new[] { 1, 2, 3 }),
            //    new With<int[]>(new[] { 0, 2, 3 })),
            //new TestCaseData(
            //    new With<int[]>(new[] { 1, 2 }),
            //    new With<int[]>(new[] { 1, 2, 3 })),
            //new TestCaseData(
            //    new With<int[]>(new[] { 1, 2 }),
            //    new With<int[]>(null)),
            //new TestCaseData(
            //    new With<int[]>(new int[0]),
            //    new With<int[]>(null)),

            //new TestCaseData(
            //    new With<List<int>>(new List<int> { 1, 2, 3 }),
            //    new With<List<int>>(new List<int> { 1, 2, 4 })),
            //new TestCaseData(
            //    new With<List<int>>(new List<int> { 1, 2, 3 }),
            //    new With<List<int>>(new List<int> { 0, 2, 3 })),
            //new TestCaseData(
            //    new With<List<int>>(new List<int> { 1, 2 }),
            //    new With<List<int>>(new List<int> { 1, 2, 3 })),
            //new TestCaseData(
            //    new With<List<int>>(new List<int> { 1, 2 }),
            //    new With<List<int>>(null)),
            //new TestCaseData(
            //    new With<List<int>>(new List<int>()),
            //    new With<List<int>>(null)),

            //new TestCaseData(
            //    new With<IReadOnlyList<int>>(new[] { 1, 2, 3 }),
            //    new With<IReadOnlyList<int>>(new[] { 1, 2, 4 })),
            //new TestCaseData(
            //    new With<IReadOnlyList<int>>(new[] { 1, 2, 3 }),
            //    new With<IReadOnlyList<int>>(new[] { 0, 2, 3 })),
            //new TestCaseData(
            //    new With<IReadOnlyList<int>>(new[] { 1, 2 }),
            //    new With<IReadOnlyList<int>>(new[] { 1, 2, 3 })),
            //new TestCaseData(
            //    new With<IReadOnlyList<int>>(new[] { 1, 2 }),
            //    new With<IReadOnlyList<int>>(null)),
            //new TestCaseData(
            //    new With<IReadOnlyList<int>>(new int[0]),
            //    new With<IReadOnlyList<int>>(null)),

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
    }
}