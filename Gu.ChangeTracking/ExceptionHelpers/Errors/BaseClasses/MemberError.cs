namespace Gu.ChangeTracking
{
    using System;
    using System.Linq;
    using System.Reflection;

    internal abstract class MemberError : Error
    {
        protected MemberError(MemberInfo member, MemberPath path)
        {
            this.Path = path;
            this.MemberInfo = member;
        }

        public MemberInfo MemberInfo { get; }

        public MemberPath Path { get; }

        public Type SourceType()
        {
            var last = this.Path.OfType<IMemberItem>()
                                    .LastOrDefault();
            if (last != null)
            {
                return last.Member.DeclaringType;
            }

            return this.Path.Root.Type;
        }
    }
}