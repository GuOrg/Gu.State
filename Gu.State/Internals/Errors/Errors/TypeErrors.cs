namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{GetType().Name} Type: {Type?.Name}")]
    internal sealed class TypeErrors : Error, IWithErrors
    {
        private static readonly Error[] EmptyErrors = new Error[0];
        private IReadOnlyList<Error> lazyAllErrors;

        internal TypeErrors(Type type)
            : this(type, EmptyErrors)
        {
        }

        internal TypeErrors(Type type, params Error[] errors)
            : this(type, (IReadOnlyList<Error>)errors)
        {
        }

        internal TypeErrors(Type type, IReadOnlyList<Error> errors)
        {
            this.Type = type;
            this.Errors = errors;
        }

        public IReadOnlyList<Error> Errors { get; }

        internal Type Type { get; }

        internal IReadOnlyList<Error> AllErrors => this.lazyAllErrors ??= MergedErrors.MergeAll(this, this.Errors);

        internal static TypeErrors Create(ErrorBuilder.TypeErrorsBuilder builder) => new(builder.Type, builder.Errors);
    }
}
