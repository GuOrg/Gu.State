namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal static partial class Set
    {
        internal interface ISortedByHashCode : IReadOnlyList<object>
        {
            bool HasCollision { get; }

            bool HashesEquals(ISortedByHashCode other);

            Indices MatchingHashIndices(object xi);

            void RemoveAt(int index);
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

            public bool HashesEquals(ISortedByHashCode other)
            {
                var sortedByHashCode = other as SortedByHashCode<T>;
                if (sortedByHashCode == null)
                {
                    throw Gu.State.Throw.ShouldNeverGetHereException("Must be the same type here");
                }

                if (this.sortedItems.Count != sortedByHashCode.Count)
                {
                    return false;
                }

                for (int i = 0; i < this.sortedItems.Count; i++)
                {
                    if (this.sortedItems[i].HashCode != sortedByHashCode.sortedItems[i].HashCode)
                    {
                        return false;
                    }
                }

                return true;
            }

            public Indices MatchingHashIndices(object xi)
            {
                if (this.comparer == null)
                {
                    return new Indices(0, this.sortedItems.Count - 1);
                }

                var hashCode = this.comparer.GetHashCode((T)xi);
                var index = this.sortedItems.BinarySearch(new Hashed<T>(hashCode, (T)xi));
                if (index < 0)
                {
                    return new Indices(0, 0);
                }

                while (index > 0 && this.sortedItems[index - 1].HashCode == hashCode)
                {
                    index--;
                }

                var start = index;
                while (index < this.sortedItems.Count - 1 && this.sortedItems[index + 1].HashCode == hashCode)
                {
                    index++;
                }

                return new Indices(start, index);
            }

            public void RemoveAt(int index)
            {
                this.sortedItems.RemoveAt(index);
            }

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

        internal struct Indices
        {
            internal readonly int First;
            internal readonly int Last;

            public Indices(int first, int last)
            {
                this.First = first;
                this.Last = last;
            }
        }
    }
}
