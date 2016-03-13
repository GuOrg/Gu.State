namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    [DebuggerDisplay("{GetType().Name} Member: {Path.LastMember.DeclaringType.Name}.{Path.LastMember.Name}")]
    internal sealed class MemberErrors : Error, IWithErrors, IExcludableMember, INotsupportedMember
    {
        private static readonly Error[] EmptyErrors = new Error[0];

        public MemberErrors(MemberInfo memberInfo)
        {
            this.Path = new MemberPath(null).WithMember(memberInfo);
            this.Errors = EmptyErrors;
        }

        public MemberErrors(MemberPath path, TypeErrors errors)
        {
            this.Path = path;
            this.Errors = new[] { errors };
        }

        public MemberPath Path { get; }

        public MemberInfo Member => this.Path.LastMember;

        public IReadOnlyCollection<Error> Errors { get; }
    }
}