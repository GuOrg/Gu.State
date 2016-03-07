namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class TypeErrors : Error, IErrors
    {
        private readonly List<Error> errors = new List<Error>();

        public TypeErrors(Type type)
        {
            this.Type = type;
        }

        internal Type Type { get; }

        private IEnumerable<Error> AllErrors => this.errors.Concat(this.errors.OfType<IErrors>().SelectMany(es => es));

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