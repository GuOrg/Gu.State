﻿namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal static partial class ErrorBuilder
    {
        internal sealed class TypeErrorsBuilder : Error
        {
            private readonly List<Error> errors = new List<Error>();

            internal TypeErrorsBuilder(Type type)
            {
                this.Type = type;
            }

            public Type Type { get; }

            public IReadOnlyList<Error> Errors => this.errors;

            internal TypeErrorsBuilder Add(Error error)
            {
                this.errors.Add(error);
                return this;
            }
        }
    }
}
