namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    internal class TypeErrors : Error, IEnumerable<Error>
    {
        public TypeErrors(Type type)
        {
            this.Type = type;
        }

        internal Type Type { get; }

        internal List<Error> Errors { get; } = new List<Error>();

        public IEnumerator<Error> GetEnumerator()
        {
            return this.Errors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}