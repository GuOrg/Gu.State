namespace Gu.State
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class MergedErrors : IReadOnlyList<Error>
    {
        private readonly List<Error> errors;

        internal MergedErrors(IEnumerable<Error> errors)
        {
            this.errors = new List<Error>();
            foreach (var error in errors)
            {
                this.Merge(error);
            }
        }

        internal MergedErrors(IEnumerable<Error> first, IEnumerable<Error> other)
            : this(first.Concat(other))
        {
        }

        public int Count => this.errors.Count;

        public Error this[int index] => this.errors[index];

        public IEnumerator<Error> GetEnumerator() => this.errors.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void Merge(Error error)
        {
            if (this.TryMerge(error as TypeErrors) || this.TryMerge(error as MemberErrors))
            {
                return;
            }

            if (this.errors.Contains(error))
            {
                return;
            }

            this.errors.Add(error);
        }

        private bool TryMerge(TypeErrors error)
        {
            if (error == null)
            {
                return false;
            }

            var match = this.errors.OfType<TypeErrors>().SingleOrDefault(e => e.Type == error.Type);
            if (match != null)
            {
                this.errors.Remove(match);
                var merged = match.Merge(error);
                this.errors.Add(merged);
                return true;
            }

            this.errors.Add(error);
            return true;
        }

        private bool TryMerge(MemberErrors error)
        {
            if (error == null)
            {
                return false;
            }

            var match = this.errors.OfType<MemberErrors>().SingleOrDefault(e => e.Member == error.Member);
            if (match != null)
            {
                this.errors.Remove(match);
                var mergedTypeErrors = match.Error.Merge(error.Error);
                var merged = new MemberErrors(match.Path, mergedTypeErrors);
                this.errors.Add(merged);
                return true;
            }

            this.errors.Add(error);
            return true;
        }

        public static IReadOnlyList<Error> MergeAll(TypeErrors typeErrors, IReadOnlyCollection<Error> errors)
        {
            var allErrors = new List<Error> { typeErrors };
            Add(errors, allErrors);
            return allErrors;
        }

        private static void Add(IReadOnlyCollection<Error> errors, List<Error> allErrors, List<IWithErrors> withErrors = null)
        {
            if (errors == null)
            {
                return;
            }

            foreach (var error in errors)
            {
                if (!allErrors.Contains(error, ErrorComparer.Default))
                {
                    allErrors.Add(error);
                }

                var we = error as IWithErrors;
                if (we != null)
                {
                    if (withErrors == null)
                    {
                        withErrors = new List<IWithErrors>();
                    }
                    else
                    {
                        if (withErrors.Any(e => ReferenceEquals(e, error)))
                        {
                            continue;
                        }
                    }

                    withErrors.Add(we);
                    Add(we.Errors, allErrors, withErrors);
                }
            }
        }

        private class ErrorComparer : IEqualityComparer<Error>
        {
            public static readonly ErrorComparer Default = new ErrorComparer();

            private ErrorComparer()
            {
            }

            public bool Equals(Error x, Error y)
            {
                bool equals;
                if (TryEquals(x as TypeErrors, y as TypeErrors, out equals) ||
                    TryEquals(x as MemberErrors, y as MemberErrors, out equals) ||
                    TryEquals(x as UnsupportedIndexer, y as UnsupportedIndexer, out equals))
                {
                    return equals;
                }

                return x == y;
            }

            public int GetHashCode(Error obj)
            {
                return 0;
            }

            private static bool TryEquals(TypeErrors x, TypeErrors y, out bool result)
            {
                if (x == null || y == null)
                {
                    result = false;
                    return false;
                }

                result = x.Type == y.Type;
                return true;
            }

            private static bool TryEquals(MemberErrors x, MemberErrors y, out bool result)
            {
                if (x == null || y == null)
                {
                    result = false;
                    return false;
                }

                result = x.Member == y.Member;
                return true;
            }

            private static bool TryEquals(UnsupportedIndexer x, UnsupportedIndexer y, out bool result)
            {
                if (x == null || y == null)
                {
                    result = false;
                    return false;
                }

                result = x.Member == y.Member;
                return true;
            }
        }
    }
}
