// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable RedundantCast
#pragma warning disable CA1825 // Avoid zero-length array allocations.
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
#pragma warning disable SA1202 // Elements should be ordered by access
namespace Gu.State.Tests.EqualByTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using NUnit.Framework;

    using static EqualByTypes;
    using ImmutableArray = Gu.State.ImmutableArray;

    public static class TestCases
    {
        private static readonly WithSimpleProperties SharedWithSimpleProperties = new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries);

        public static readonly TestCaseData[] WhenEqual =
        {
            Case<object>(null, null),
            Case(1, 1),
            Case((int?)null, (int?)null),
            Case((int?)1, (int?)1),

            Case(StringComparison.Ordinal, StringComparison.Ordinal),
            Case((StringComparison?)null, (StringComparison?)null),
            Case((StringComparison?)StringComparison.Ordinal, (StringComparison?)StringComparison.Ordinal),

            Case(new Point(1, 2), new Point(1, 2)),
            Case((Point?)new Point(1, 2), (Point?)new Point(1, 2)),
            Case((Point?)null, (Point?)null),
            Case(new Struct { Value = 1 }, new Struct { Value = 1 }),
            Case(new EquatableStruct { Value = 1 }, new EquatableStruct { Value = 1 }),

            Case(new With<int>(1), new With<int>(1)),
            Case(new With<int?>(1), new With<int?>(1)),
            Case(new With<int?>(null), new With<int?>(null)),

            Case(new With<Struct>(new Struct { Value = 1 }), new With<Struct>(new Struct { Value = 1 })),
            Case(new With<Struct?>(new Struct { Value = 1 }), new With<Struct?>(new Struct { Value = 1 })),
            Case(new With<Struct?>(null), new With<Struct?>(null)),
            Case(new With<EquatableStruct>(new EquatableStruct { Value = 1 }), new With<EquatableStruct>(new EquatableStruct { Value = 1 })),
            Case(new With<EquatableStruct?>(new EquatableStruct { Value = 1 }), new With<EquatableStruct?>(new EquatableStruct { Value = 1 })),
            Case(new With<EquatableStruct?>(null), new With<EquatableStruct?>(null)),

            Case(new With<StringComparison>(StringComparison.Ordinal), new With<StringComparison>(StringComparison.Ordinal)),
            Case(new With<StringComparison?>(StringComparison.Ordinal), new With<StringComparison?>(StringComparison.Ordinal)),
            Case(new With<StringComparison?>(null), new With<StringComparison?>(null)),

            Case(new With<Guid>(Guid.Parse("f062db24-d4b8-452a-904c-ba2d23663e92")), new With<Guid>(Guid.Parse("f062db24-d4b8-452a-904c-ba2d23663e92"))),

            Case(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries)),
            Case(new WithSimpleValues(1, 2, null, StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, 2, null, StringSplitOptions.RemoveEmptyEntries)),
            Case(new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries)),

            Case(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries)),
            Case(new WithSimpleProperties(1, 2, null, StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, 2, null, StringSplitOptions.RemoveEmptyEntries)),
            Case(new WithSimpleProperties(1, null, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, null, "3", StringSplitOptions.RemoveEmptyEntries)),
            Case(new Derived1 { BaseValue = 1, Derived1Value = 2 }, new Derived1 { BaseValue = 1, Derived1Value = 2 }),

            Case((int[])null, (int[])null),
            Case(new int[0], new int[0]),
            Case(new[] { 1, 2, 3 }, new[] { 1, 2, 3 }),
            Case(new[] { SharedWithSimpleProperties }, new[] { SharedWithSimpleProperties }),
            Case(new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } }, new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } }),

            Case(new List<int>(), new List<int>()),
            Case(new List<int> { 1, 2, 3 }, new List<int> { 1, 2, 3 }),
            Case(new List<Point> { new Point(1, 2), new Point(1, 2) }, new List<Point> { new Point(1, 2), new Point(1, 2) }),

            Case(new HashSet<int>(), new HashSet<int>()),
            Case(new HashSet<int>(new[] { 1, 2, 3 }), new HashSet<int>(new[] { 1, 2, 3 })),
            Case(new HashSet<int>(new[] { 1, 2, 3 }), new HashSet<int>(new[] { 3, 2, 1 })),
            Case(new HashSet<WithSimpleProperties>(new[] { SharedWithSimpleProperties }), new HashSet<WithSimpleProperties>(new[] { SharedWithSimpleProperties })),

            Case(new ObservableCollection<int>(), new ObservableCollection<int>()),
            Case(new ObservableCollection<int> { 1, 2, 3 }, new ObservableCollection<int> { 1, 2, 3 }),
            Case(new ObservableCollection<Point> { new Point(1, 2), new Point(1, 2) }, new ObservableCollection<Point> { new Point(1, 2), new Point(1, 2) }),

            Case(new Dictionary<int, string> { { 1, "1" } }, new Dictionary<int, string> { { 1, "1" } }),
            Case(new Dictionary<int, WithSimpleProperties> { { 1, SharedWithSimpleProperties } }, new Dictionary<int, WithSimpleProperties> { { 1, SharedWithSimpleProperties } }),

            Case(new object[] { 1, 1.2, "3" }.Select(x => x), new object[] { 1, 1.2, "3" }.Select(x => x)),
            Case(new object[] { 1, 2.2, "3" }, new object[] { 1, 2.2, "3" }),
            Case(new List<object> { 1, 2.2, "3" }, new List<object> { 1, 2.2, "3" }),
            Case(new Dictionary<object, object> { { 1, "1" } }, new Dictionary<object, object> { { 1, "1" } }),
            Case(new Dictionary<object, string> { { 1, "1" } }, new Dictionary<object, string> { { 1, "1" } }),
            Case(new HashSet<object> { 1, 2.2, "3" }, new HashSet<object> { 1, 2.2, "3" }),

            //Case(default(ImmutableArray<int>), default(ImmutableArray<int>)),
            Case(ImmutableList.Create(1, 2, 3), ImmutableList.Create(1, 2, 3)),
            Case(ImmutableArray<int>.Empty, ImmutableArray<int>.Empty),
            Case(ImmutableArray.Create(new[] { 1, 2, 3 }), ImmutableArray.Create(new[] { 1, 2, 3 })),
            Case(ImmutableSortedSet.Create(new[] { 1, 2, 3 }), ImmutableSortedSet.Create(new[] { 1, 2, 3 })),
            Case(ImmutableHashSet.Create(new[] { 1, 2, 3 }), ImmutableHashSet.Create(new[] { 1, 2, 3 })),
            Case(ImmutableStack.Create(new[] { 1, 2, 3 }), ImmutableStack.Create(new[] { 1, 2, 3 })),
            Case(ImmutableQueue.Create(new[] { 1, 2, 3 }), ImmutableQueue.Create(new[] { 1, 2, 3 })),
            Case(ImmutableDictionary.CreateRange(new Dictionary<int, string> { { 1, "1" } }), ImmutableDictionary.CreateRange(new Dictionary<int, string> { { 1, "1" } })),

            Case(Enumerable.Empty<int>().Select(x => x * x), Enumerable.Empty<int>().Select(x => x * x)),
            Case("1,2".Split(',').Select(int.Parse), "1,2".Split(',').Select(int.Parse)),
            Case("1,2".Split(',').Select(int.Parse), "1,2".Split(',').Select(int.Parse)),
            Case(new object[] { 1, null }.Select(x => x), new object[] { 1, null }.Select(x => x)),

            Case(new With<Point>(new Point(1, 2)), new With<Point>(new Point(1, 2))),
            Case(new With<Point?>(new Point(1, 2)), new With<Point?>(new Point(1, 2))),
            Case(new With<Point?>(null), new With<Point?>(null)),

            Case(new WithComplexProperty("a", 1), new WithComplexProperty("a", 1)),
            Case(new WithListProperty<int> { Items = null }, new WithListProperty<int> { Items = null }),
            Case(new With<WithSimpleProperties>(SharedWithSimpleProperties), new With<WithSimpleProperties>(SharedWithSimpleProperties)),
        };

        public static readonly TestCaseData[] WhenEqualStructural =
        {
            Case(new With<int[]>(null), new With<int[]>(null)),
            Case(new With<int[]>(new int[0]), new With<int[]>(new int[0])),
            Case(new With<int[]>(new[] { 1, 2, 3 }), new With<int[]>(new[] { 1, 2, 3 })),

            Case(new With<List<int>>(null), new With<List<int>>(null)),
            Case(new With<List<int>>(new List<int>()), new With<List<int>>(new List<int>())),
            Case(new With<List<int>>(new List<int> { 1, 2, 3 }), new With<List<int>>(new List<int> { 1, 2, 3 })),
            Case(new With<List<object>>(new List<object> { 1, 2.2, 3 }), new With<List<object>>(new List<object> { 1, 2.2, 3 })),

            Case(new With<IReadOnlyList<int>>(null), new With<IReadOnlyList<int>>(null)),
            Case(new With<IReadOnlyList<int>>(new int[0]), new With<IReadOnlyList<int>>(new int[0])),
            Case(new With<IReadOnlyList<int>>(new[] { 1, 2, 3 }), new With<IReadOnlyList<int>>(new[] { 1, 2, 3 })),
            Case(new With<IReadOnlyList<object>>(new object[] { 1, 2.2, 3 }), new With<IReadOnlyList<object>>(new object[] { 1, 2.2, 3 })),

            Case(new[] { new[] { 1, 2, 3 }, new[] { 4, 5 } }, new[] { new[] { 1, 2, 3 }, new[] { 4, 5 } }),
            Case(new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) }, new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) }),
            Case(new HashSet<HashCollisionType> { new HashCollisionType { Value = 1 } }, new HashSet<HashCollisionType> { new HashCollisionType { Value = 1 } }),
            Case(new Dictionary<HashCollisionType, string> { { new HashCollisionType { Value = 1 }, "1" } }, new Dictionary<HashCollisionType, string> { { new HashCollisionType { Value = 1 }, "1" } }),

            Case(new With<BaseClass>(new Derived1 { BaseValue = 1, Derived1Value = 2 }), new With<BaseClass>(new Derived1 { BaseValue = 1, Derived1Value = 2 })),
            Case(new[] { new With<int>(1), new With<int>(2), new With<int>(3) }, new[] { new With<int>(1), new With<int>(2), new With<int>(3) }),
            Case(new WithComplexProperty("a", 1) { ComplexType = new ComplexType { Name = "1", Value = 2 } }, new WithComplexProperty("a", 1) { ComplexType = new ComplexType { Name = "1", Value = 2 } }),
            Case(new With<ComplexType>(new ComplexType("1", 2)), new With<ComplexType>(new ComplexType("1", 2))),
            Case(new WithListProperty<int> { Items = new List<int>() }, new WithListProperty<int> { Items = new List<int>() }),
            Case(new ObservableCollection<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) }, new ObservableCollection<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) }),
        };

        public static readonly TestCaseData[] WhenNotEqual =
        {
            Case(null, (object)1),
            Case((object)1,  (object)2),
            Case(1, 2),
            Case((int?)null, (int?)1),
            Case((int?)null, (int?)1),

            Case(new Struct { Value = 1 }, new Struct { Value = -1 }),
            Case(new EquatableStruct { Value = 1 }, new EquatableStruct { Value = -1 }),

            Case(StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase),
            Case((StringComparison?)null, (StringComparison?)StringComparison.Ordinal),
            Case((StringComparison?)StringComparison.Ordinal, (StringComparison?)StringComparison.OrdinalIgnoreCase),

            Case(new Point(1, 2), new Point(1, -2)),
            Case(new Point(1, 2), new Point(-1, 2)),
            Case((Point?)new Point(1, 2), (Point?)new Point(1, -2)),
            Case((Point?)new Point(1, 2), (Point?)new Point(-1, 2)),
            Case((Point?)new Point(1, 2), (Point?)null),

            Case(new With<int>(1), new With<int>(2)),
            Case(new With<int?>(1), new With<int?>(2)),
            Case(new With<int?>(1), new With<int?>(null)),
            Case(new With<int>(1), (With<int>)null),

            Case(new With<Struct>(new Struct { Value = 1 }), new With<Struct>(new Struct { Value = -1 })),
            Case(new With<Struct?>(new Struct { Value = 1 }), new With<Struct?>(new Struct { Value = -1 })),
            Case(new With<Struct?>(null), new With<Struct?>(new Struct { Value = -1 })),

            Case(new With<EquatableStruct>(new EquatableStruct { Value = 1 }), new With<EquatableStruct>(new EquatableStruct { Value = -1 })),
            Case(new With<EquatableStruct?>(new EquatableStruct { Value = 1 }), new With<EquatableStruct?>(new EquatableStruct { Value = -1 })),
            Case(new With<EquatableStruct?>(null), new With<EquatableStruct?>(new EquatableStruct { Value = -1 })),

            Case(new With<Point>(new Point(1, 2)), new With<Point>(new Point(1, -2))),
            Case(new With<Point>(new Point(1, 2)), new With<Point>(new Point(-1, 2))),
            Case(new With<Point?>(new Point(1, 2)), new With<Point?>(new Point(1, -2))),
            Case(new With<Point?>(new Point(1, 2)), new With<Point?>(null)),

            Case(new With<StringComparison>(StringComparison.Ordinal), new With<StringComparison>(StringComparison.OrdinalIgnoreCase)),
            Case(new With<StringComparison?>(StringComparison.Ordinal), new With<StringComparison?>(StringComparison.OrdinalIgnoreCase)),
            Case(new With<StringComparison?>(StringComparison.Ordinal), new With<StringComparison?>(null)),

            Case(new With<Guid>(Guid.Parse("f062db24-d4b8-452a-904c-ba2d23663e92")), new With<Guid>(Guid.Parse("f062db24-d4b8-452a-904c-ba2d23663e93"))),

            Case(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(5, 2, "3", StringSplitOptions.RemoveEmptyEntries)),
            Case(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, 5, "3", StringSplitOptions.RemoveEmptyEntries)),
            Case(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries)),
            Case(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, 5, null, StringSplitOptions.RemoveEmptyEntries)),
            Case(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, 2, "5", StringSplitOptions.RemoveEmptyEntries)),
            Case(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleValues(1, 2, "3", StringSplitOptions.None)),

            Case(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(5, 2, "3", StringSplitOptions.RemoveEmptyEntries)),
            Case(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, 5, "3", StringSplitOptions.RemoveEmptyEntries)),
            Case(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, null, "3", StringSplitOptions.RemoveEmptyEntries)),
            Case(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, 5, null, StringSplitOptions.RemoveEmptyEntries)),
            Case(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, 2, "5", StringSplitOptions.RemoveEmptyEntries)),
            Case(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries), new WithSimpleProperties(1, 2, "3", StringSplitOptions.None)),

            Case(new Derived1 { BaseValue = 1, Derived1Value = 2 }, new Derived1 { BaseValue = -1, Derived1Value = 2 }),
            Case(new Derived1 { BaseValue = 1, Derived1Value = 2 }, new Derived1 { BaseValue = 1, Derived1Value = -2 }),

            Case((int[])null, new int[0]),
            Case((int[])null, new[] { 1, 2, 3 }),
            Case(new int[0], new[] { 1, 2, 3 }),
            Case(new[] { 1, 2, 3 }, new[] { 1, 2 }),
            Case(new[] { 1, 2, 3 }, new[] { 1, 2, -1 }),
            Case(new[] { 1, 2, 3 }, new[] { -1, 2, 3 }),

            Case(new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } }, new[,] { { -1, 2 }, { 3, 4 }, { 5, 6 } }),
            Case(new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } }, new[,] { { 1, 2 }, { -3, 4 }, { 5, 6 } }),
            Case(new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } }, new[,] { { 1, 2 }, { 3, 4 }, { 5, -6 } }),

            Case(new List<int> { 1, 2, 3 }, new List<int> { -1, 2, 3 }),
            Case(new List<int> { 1, 2, 3 }, new List<int> { 1, -2, 3 }),
            Case(new List<int> { 1, 2, 3 }, new List<int> { 1, 2, -3 }),
            Case(new List<int> { 1, 2, 3 }, new List<int> { 1, 2 }),
            Case(new List<int>(), new List<int> { 1, 2, 3 }),

            Case(new HashSet<int> { 1, 2, 3 }, new HashSet<int>()),
            Case(new HashSet<int> { 1, 2, 3 }, new HashSet<int> { -1, 2, 3 }),
            Case(new HashSet<int> { 1, 2, 3 }, new HashSet<int> { 1, -2, 3 }),
            Case(new HashSet<int> { 1, 2, 3 }, new HashSet<int> { 1, 2, -3 }),
            Case(new HashSet<int> { 1, 2, 3 }, new HashSet<int> { 1, 2 }),

            Case(new ObservableCollection<int> { 1, 2, 3 }, new ObservableCollection<int> { -1, 2, 3 }),
            Case(new ObservableCollection<int> { 1, 2, 3 }, new ObservableCollection<int> { 1, -2, 3 }),
            Case(new ObservableCollection<int> { 1, 2, 3 }, new ObservableCollection<int> { 1, 2, -3 }),
            Case(new ObservableCollection<int> { 1, 2, 3 }, new ObservableCollection<int> { 1, 2 }),
            Case(new ObservableCollection<int>(), new ObservableCollection<int> { 1, 2, 3 }),

            Case(new Dictionary<int, string> { { 1, "1" } }, new Dictionary<int, string>()),
            Case(new Dictionary<int, string> { { 1, "1" }, { 2, "2" } }, new Dictionary<int, string> { { 1, "1" } }),
            Case(new Dictionary<int, string> { { 1, "1" } }, new Dictionary<int, string> { { -1, "1" } }),
            Case(new Dictionary<int, string> { { 1, "1" } }, new Dictionary<int, string> { { 1, "-1" } }),

            Case(ImmutableList.Create(1, 2, 3), ImmutableList.Create(1, 2, -3)),
            Case(ImmutableArray.Create(new[] { 1, 2, 3 }), ImmutableArray.Create(new[] { 1, 2, -3 })),
            Case(ImmutableHashSet.Create(new[] { 1, 2, 3 }), ImmutableHashSet.Create(new[] { 1, 2, -3 })),
            Case(ImmutableSortedSet.Create(new[] { 1, 2, 3 }), ImmutableSortedSet.Create(new[] { 1, 2, -3 })),
            Case(ImmutableHashSet.Create(new[] { 1, 2, 3 }), ImmutableHashSet.Create(new[] { 1, 2, -3 })),
            Case(ImmutableStack.Create(new[] { 1, 2, 3 }), ImmutableStack.Create(new[] { 1, 2, -3 })),
            Case(ImmutableQueue.Create(new[] { 1, 2, 3 }), ImmutableQueue.Create(new[] { 1, 2, -3 })),
            Case(ImmutableDictionary.CreateRange(new Dictionary<int, string> { { 1, "1" } }), ImmutableDictionary.CreateRange(new Dictionary<int, string> { { -1, "1" } })),
            Case(ImmutableDictionary.CreateRange(new Dictionary<int, string> { { 1, "1" } }), ImmutableDictionary.CreateRange(new Dictionary<int, string> { { 1, "-1" } })),

            Case(new[] { 1 }.Select(x => x * x), Enumerable.Empty<int>().Select(x => x * x)),
            Case("1,2".Split(',').Select(int.Parse), "1,-2".Split(',').Select(int.Parse)),
            Case("1,2".Split(',').Select(int.Parse), "-1,2".Split(',').Select(int.Parse)),
        };

        public static readonly TestCaseData[] WhenNotEqualStructural =
        {
            Case(new With<object>(1), new With<object>(2)),
            Case(new With<object>(1), new With<object>(null)),

            Case(new With<int[]>(new[] { 1, 2, 3 }), new With<int[]>(new[] { 1, 2, 4 })),
            Case(new With<int[]>(new[] { 1, 2, 3 }), new With<int[]>(new[] { 0, 2, 3 })),
            Case(new With<int[]>(new[] { 1, 2 }), new With<int[]>(new[] { 1, 2, 3 })),
            Case(new With<int[]>(new[] { 1, 2 }), new With<int[]>(null)),
            Case(new With<int[]>(new int[0]), new With<int[]>(null)),

            Case(new With<List<int>>(new List<int> { 1, 2, 3 }), new With<List<int>>(new List<int> { 1, 2, 4 })),
            Case(new With<List<int>>(new List<int> { 1, 2, 3 }), new With<List<int>>(new List<int> { 0, 2, 3 })),
            Case(new With<List<int>>(new List<int> { 1, 2 }), new With<List<int>>(new List<int> { 1, 2, 3 })),
            Case(new With<List<int>>(new List<int> { 1, 2 }), new With<List<int>>(null)),
            Case(new With<List<int>>(new List<int>()), new With<List<int>>(null)),

            Case(new With<IReadOnlyList<int>>(new[] { 1, 2, 3 }), new With<IReadOnlyList<int>>(new[] { 1, 2, 4 })),
            Case(new With<IReadOnlyList<int>>(new[] { 1, 2, 3 }), new With<IReadOnlyList<int>>(new[] { 0, 2, 3 })),
            Case(new With<IReadOnlyList<int>>(new[] { 1, 2 }), new With<IReadOnlyList<int>>(new[] { 1, 2, 3 })),
            Case(new With<IReadOnlyList<int>>(new[] { 1, 2 }), new With<IReadOnlyList<int>>(null)),
            Case(new With<IReadOnlyList<int>>(new int[0]), new With<IReadOnlyList<int>>(null)),

            Case(new object[] { 1, 2.2 }, new object[] { -1, 2.2 }),
            Case(new object[] { 1, 2.2 }, new object[] { 1, -2.2 }),
            Case(new[] { new[] { 1, 2, 3 }, new[] { 4, 5 } }, new[] { new[] { 1, 2 }, new[] { 4, 5 } }),
            Case(new[] { new[] { 1, 2, 3 }, new[] { 4, 5 } }, new[] { new[] { 1, 2, 3 }, new[] { 4 } }),
            Case(new[] { new[] { 1, 2, 3 }, new[] { 4, 5 } }, new[] { new[] { -1, 2, 3 }, new[] { 4, 5 } }),
            Case(new[] { new[] { 1, 2, 3 }, new[] { 4, 5 } }, new[] { new[] { 1, 2, 3 }, new[] { 4, -5 } }),

            Case(new HashSet<HashCollisionType> { new HashCollisionType { Value = 1 } }, new HashSet<HashCollisionType> { new HashCollisionType { Value = -1 } }),

            Case(new Dictionary<HashCollisionType, string> { { new HashCollisionType { Value = 1 }, "1" } }, new Dictionary<HashCollisionType, string> { { new HashCollisionType { Value = -1 }, "1" } }),

            Case(new[] { new With<int>(1), new With<int>(2), new With<int>(3) }, new[] { new With<int>(4), new With<int>(5), new With<int>(6) }),

            Case(new With<BaseClass>(new Derived1 { BaseValue = 1, Derived1Value = 2 }), new With<BaseClass>(new Derived1 { BaseValue = 1, Derived1Value = -2 })),
            Case(new With<BaseClass>(new Derived1()), new With<BaseClass>(new Derived2())),
            Case(new object[] { 1, null }.Select(x => x), new object[] { null, 1 }.Select(x => x)),
            Case(new object[] { 1, null }.Select(x => x), new object[] { 1 }.Select(x => x)),

            Case(new WithComplexProperty("a", 1) { ComplexType = new ComplexType { Name = "1", Value = 2 } }, new WithComplexProperty("a", -1) { ComplexType = new ComplexType { Name = "1", Value = 2 } }),
            Case(new WithComplexProperty("a", 1) { ComplexType = new ComplexType { Name = "1", Value = 2 } }, new WithComplexProperty("a", 1) { ComplexType = new ComplexType { Name = "1", Value = -2 } }),
            Case(new WithComplexProperty("a", 1) { ComplexType = new ComplexType { Name = "1", Value = 2 } }, new WithComplexProperty("a", 1)),
            Case(new With<ComplexType>(new ComplexType("1", 2)), new With<ComplexType>(new ComplexType("1", -2))),
            Case(new WithListProperty<int> { Items = { 1, 2, 3 } },  new WithListProperty<int>()),
            Case(new WithListProperty<int> { Items = { 1, 2, 3 } },  new WithListProperty<int> { Items = new List<int>() }),
            Case(new ObservableCollection<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) }, new ObservableCollection<ComplexType> { new ComplexType("b", 2), new ComplexType("c", -3) }),
        };

        private static TestCaseData Case<T>(T x, T y) => new TestCaseData(x, y);

        private static With<T> With<T>(T x) => new With<T>(x);
    }
}