namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [DebuggerDisplay("{GetType().Name} Type: {Type.Name}")]
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

        internal TypeErrors(Type type, IReadOnlyCollection<Error> errors)
        {
            this.Type = type;
            this.Errors = errors;
            var allErrors = new HashSet<Error> { this };
            GetAllErrors(errors, allErrors);
            allErrors.Add(this);
            this.AllErrors = allErrors.ToList();
        }

        internal Type Type { get; }

        public IReadOnlyCollection<Error> Errors { get; }

        internal IReadOnlyCollection<Error> AllErrors { get; }

        private static void GetAllErrors(
            IReadOnlyCollection<Error> errors,
            HashSet<Error> allErrors)
        {
            if (errors == null)
            {
                return;
            }

            foreach (var error in errors)
            {
                if (!allErrors.Add(error))
                {
                    continue;
                }

                var withErrors = error as IWithErrors;
                GetAllErrors(withErrors?.Errors, allErrors);
            }
        }
    }
}