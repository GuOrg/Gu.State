namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal class ErrorEqualByComparer : EqualByComparer
    {
        internal ErrorEqualByComparer(Type type, Error error)
        {
            this.Type = type;
            this.Error = error;
        }

        public Type Type { get; }

        public Error Error { get; }

        internal override bool CanHaveReferenceLoops => false;

        internal override bool TryGetError(MemberSettings settings, out Error error)
        {
            error = this.Error;
            return true;
        }

        internal override bool Equals(object x, object y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
        {
            throw Throw.CompareWhenError;
        }
    }
}
