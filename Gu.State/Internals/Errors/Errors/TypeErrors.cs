namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{GetType().Name} Type: {Type?.Name}")]
    internal sealed class TypeErrors : Error, IWithErrors
    {
        private static readonly Error[] EmptyErrors = new Error[0];

        internal TypeErrors(Type type)
            : this(type, EmptyErrors)
        {
        }

        internal TypeErrors(Type type, Error error)
            : this(type, new[] { error })
        {
        }

        internal TypeErrors(ErrorBuilder.TypeErrorsBuilder builder)
            : this(builder.Type, builder.Errors)
        {
        }

        internal TypeErrors(Type type, IReadOnlyList<Error> errors)
        {
            this.Type = type;
            this.Errors = errors;
            this.AllErrors = MergedErrors.MergeAll(this, errors);
        }

        public IReadOnlyList<Error> Errors { get; }

        internal Type Type { get; }

        internal IReadOnlyList<Error> AllErrors { get; }
    }
}