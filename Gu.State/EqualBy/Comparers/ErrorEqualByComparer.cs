namespace Gu.State
{
    using System;

    internal class ErrorEqualByComparer : EqualByComparer
    {
        internal ErrorEqualByComparer(Type type, Error error)
        {
            this.Type = type;
            this.Error = error;
        }

        public Type Type { get; }

        public Error Error { get; }

        internal override bool TryGetError(MemberSettings settings, out Error error)
        {
            error = this.Error;
            return true;
        }

        internal override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            throw Throw.CompareWhenError;
        }
    }
}
