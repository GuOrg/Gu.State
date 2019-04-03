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

        internal TypeErrors(Type type, IReadOnlyList<Error> errors)
        {
            this.Type = type;
            this.Errors = errors;
            this.AllErrors = MergedErrors.MergeAll(this, errors);
        }

        internal Type Type { get; }

        public IReadOnlyList<Error> Errors { get; }

        internal IReadOnlyList<Error> AllErrors { get; }

        internal static TypeErrors Create(ErrorBuilder.TypeErrorsBuilder builder) => new TypeErrors(builder.Type, builder.Errors);
    }
}