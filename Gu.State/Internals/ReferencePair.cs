namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    internal sealed class ReferencePair : IDisposable
    {
        private static readonly ConcurrentDictionary<ReferencePair, ReferencePair> Cache = new();
        private readonly WeakReference x;
        private readonly WeakReference y;
        private readonly int hashCode;
        private readonly object gate = new();
        private bool disposed;

        private ReferencePair(object x, object y)
        {
            Debug.Assert(x != null, "x == null");
            Debug.Assert(y != null, "y == null");
            this.x = new WeakReference(x);
            this.y = new WeakReference(y);
            this.hashCode = GetHashCode(x, y);
        }

        ~ReferencePair()
        {
            if (this.disposed || Environment.HasShutdownStarted)
            {
                return;
            }

            Cache.TryRemove(this, out _);
        }

        internal object X
        {
            get
            {
                this.VerifyNotDisposed();
                return this.x.Target;
            }
        }

        internal object Y
        {
            get
            {
                this.VerifyNotDisposed();
                return this.y.Target;
            }
        }

        public static bool operator ==(ReferencePair left, ReferencePair right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ReferencePair left, ReferencePair right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is ReferencePair other &&
                   this.Equals(other);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            Debug.Assert(!this.disposed, "this.disposed");
            return this.hashCode;
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            lock (this.gate)
            {
                if (this.disposed)
                {
                    return;
                }

                GC.SuppressFinalize(this);
                Cache.TryRemove(this, out _);
                this.disposed = true;
            }
        }

        internal static IRefCounted<ReferencePair> GetOrCreate<T>(T x, T y)
            where T : class
        {
            var key = new ReferencePair(x, y);
            if (!Cache.GetOrAdd(key, p => p).TryRefCount(out var refcounted))
            {
                if (!Cache.TryAdd(key, key))
                {
                    throw Throw.ShouldNeverGetHereException("Adding created pair failed");
                }

                if (!key.TryRefCount(out refcounted))
                {
                    throw Throw.ShouldNeverGetHereException("Refcounting created pair failed");
                }
            }

            return refcounted;
        }

        internal bool Equals(ReferencePair other)
        {
            Debug.Assert(!this.disposed, "this.disposed");
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ReferenceEquals(this.X, other.X) && ReferenceEquals(this.Y, other.Y);
        }

        private static int GetHashCode(object x, object y)
        {
            unchecked
            {
                return (RuntimeHelpers.GetHashCode(x) * 397) ^ RuntimeHelpers.GetHashCode(y);
            }
        }

        private void VerifyNotDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(typeof(ReferencePair).FullName);
            }
        }
    }
}
