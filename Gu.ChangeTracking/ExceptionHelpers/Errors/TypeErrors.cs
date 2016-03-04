namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class TypeErrors : Error, IEnumerable<Error>
    {
        private readonly List<Error> errors = new List<Error>();

        public TypeErrors(Type type)
        {
            this.Type = type;
        }

        internal Type Type { get; }

        internal IEnumerable<Error> AllErrors => this.errors.Concat(this.errors.OfType<TypeErrors>().SelectMany(x => x.AllErrors));

        public IEnumerator<Error> GetEnumerator()
        {
            return this.AllErrors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal TypeErrors Add(Error error)
        {
            this.errors.Add(error);
            return this;
        }
    }
}