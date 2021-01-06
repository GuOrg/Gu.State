namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class PaddedPairs : IEnumerable<PaddedPairs.Pair<object>>
    {
        internal static readonly object MissingItem = new Missing();

        private readonly IEnumerable x;
        private readonly IEnumerable y;

        internal PaddedPairs(IEnumerable x, IEnumerable y)
        {
            this.x = x;
            this.y = y;
        }

        public IEnumerator<Pair<object>> GetEnumerator()
        {
            return new PairEnumerator(this.x, this.y);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private static Pair<T> CreatePair<T>(T x, T y)
            where T : class
        {
            return new Pair<T>(x, y);
        }

        internal readonly struct Pair<T>
            where T : class
        {
            internal readonly T X;
            internal readonly T Y;

            internal Pair(T x, T y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        private sealed class PairEnumerator : IEnumerator<Pair<object>>
        {
            private readonly IEnumerator x;
            private readonly IEnumerator y;

            private bool mx;
            private bool my;

            internal PairEnumerator(IEnumerable x, IEnumerable y)
            {
                this.x = x.GetEnumerator();
                this.y = y.GetEnumerator();
            }

            public Pair<object> Current
            {
                get
                {
                    if (this.mx && this.my)
                    {
                        return CreatePair(this.x.Current, this.y.Current);
                    }
                    else if (this.mx)
                    {
                        return CreatePair(this.x.Current, MissingItem);
                    }
                    else if (this.my)
                    {
                        return CreatePair(MissingItem, this.y.Current);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            object IEnumerator.Current => this.Current;

            public void Dispose()
            {
                (this.x as IDisposable)?.Dispose();
                (this.y as IDisposable)?.Dispose();
            }

            public bool MoveNext()
            {
                this.mx = this.x.MoveNext();
                this.my = this.y.MoveNext();
                return this.mx || this.my;
            }

            public void Reset()
            {
                this.x.Reset();
                this.y.Reset();
            }
        }

        private class Missing
        {
            public override string ToString()
            {
                return "missing item";
            }
        }
    }
}
