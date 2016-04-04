namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    internal static partial class Set
    {
        internal interface ISortedByHashCode : IReadOnlyList<object>
        {
            bool HasCollision { get; }
        }

        internal class SortedByHashCode<T> : ISortedByHashCode
        {
            private readonly IEqualityComparer<T> comparer;
            private readonly List<Hashed<T>> sortedItems = new List<Hashed<T>>();
            private bool hasCollision;

            public SortedByHashCode(IEnumerable<T> source, IEqualityComparer<T> comparer)
            {
                this.comparer = comparer ?? EqualityComparer<T>.Default;
                if (comparer == null)
                {
                    this.sortedItems.AddRange(source.Select(x => new Hashed<T>(0, x)));
                }
                else
                {
                    foreach (var item in source)
                    {
                        var hashed = new Hashed<T>(comparer.GetHashCode(item), item);
                        this.Add(hashed);
                    }
                }
            }

            public int Count => this.sortedItems.Count;

            public bool HasCollision => this.hasCollision;

            public object this[int index] => this.sortedItems[index].Value;

            public IEnumerator<object> GetEnumerator() => this.sortedItems.Select(x => x.Value)
                                                              .Cast<object>()
                                                              .GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            private void Add(Hashed<T> item)
            {
                var index = this.sortedItems.BinarySearch(item);
                if (index >= 0)
                {
                    this.hasCollision = true;
                    this.sortedItems.Insert(index, item);
                }
                else
                {
                    Debug.WriteLine($"index: {index} ~index:{~index}");
                    this.sortedItems.Insert(~index, item);
                }
            }
        }

        internal struct Hashed<T> : IComparable<Hashed<T>>
        {
            internal readonly int HashCode;
            internal readonly T Value;

            public Hashed(int hashCode, T value)
            {
                this.HashCode = hashCode;
                this.Value = value;
            }

            public int CompareTo(Hashed<T> other)
            {
                return this.HashCode.CompareTo(other.HashCode);
            }
        }
    }
}
