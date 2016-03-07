namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class PaddedPairs : IEnumerable<PaddedPairs.Pair<object>>
    {
        private static readonly object NullObject = new object();
        private readonly IEnumerable x;
        private readonly IEnumerable y;

        public PaddedPairs(IEnumerable x, IEnumerable y)
        {
            this.x = x;
            this.y = y;
        }

        public IEnumerator<Pair<object>> GetEnumerator()
        {
            return new PairEnumerator(this.x.GetEnumerator(), this.y.GetEnumerator());
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

        internal struct Pair<T>
            where T : class
        {
            public readonly T X;
            public readonly T Y;

            public Pair(T x, T y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        private class PairEnumerator : IEnumerator<Pair<object>>
        {
            private readonly IEnumerator x;
            private readonly IEnumerator y;

            private bool mx;
            private bool my;

            public PairEnumerator(IEnumerator x, IEnumerator y)
            {
                this.x = x;
                this.y = y;
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
                        return CreatePair(this.x.Current, NullObject);
                    }
                    else if (this.my)
                    {
                        return CreatePair(NullObject, this.y.Current);
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
    }
}
