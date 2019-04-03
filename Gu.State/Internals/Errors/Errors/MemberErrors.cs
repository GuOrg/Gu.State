namespace Gu.State
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("{GetType().Name} Member: {Path.LastMember.DeclaringType.Name}.{Path.LastMember.Name}")]
    internal sealed class MemberErrors : Error, IWithErrors, IExcludableMember, INotsupportedMember
    {
        private static readonly Error[] EmptyErrors = new Error[0];

        public MemberErrors(MemberInfo memberInfo)
            : this(new MemberPath(null).WithMember(memberInfo), null)
        {
        }

        public MemberErrors(MemberPath path, params Error[] errors)
        {
            this.Path = path;
            this.Errors = errors;
        }

        public MemberPath Path { get; }

        public MemberInfo Member => this.Path.LastMember;

        public IReadOnlyList<Error> Errors { get; }
    }
}