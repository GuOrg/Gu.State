namespace Gu.State
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class MergedErrors : IReadOnlyCollection<Error>
    {
        private readonly List<Error> errors;

        internal MergedErrors(IEnumerable<Error> first, IEnumerable<Error> other)
        {
            this.errors = new List<Error>(first);
            foreach (var error in other)
            {
                this.Merge(error);
            }
        }

        public int Count => this.errors.Count;

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
    }
}
