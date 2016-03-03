namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Gu.ChangeTracking.Internals;

    public abstract class MemberSettings<T> : IMemberSettings
        where T : MemberInfo
    {
        private static readonly T[] EmptyMembers = new T[0];
        private readonly IgnoredTypes ignoredTypes;
        private readonly HashSet<T> ignoredMembers;

        protected MemberSettings(
            IEnumerable<T> ignoredMembers,
            IEnumerable<Type> ignoredTypes,
            BindingFlags bindingFlags,
            ReferenceHandling referenceHandling)
        {
            this.BindingFlags = bindingFlags;
            this.ReferenceHandling = referenceHandling;
            var memberset = ignoredMembers as HashSet<T>;
            if (memberset != null)
            {
                this.ignoredMembers = memberset;
            }
            else
            {
                if (ignoredMembers != null && ignoredMembers.Any())
                {
                    this.ignoredMembers = new HashSet<T>(ignoredMembers, MemberInfoComparer<T>.Default);
                }
            }

            this.ignoredTypes = IgnoredTypes.Create(ignoredTypes);
        }

        public BindingFlags BindingFlags { get; }

        public ReferenceHandling ReferenceHandling { get; }

        protected IEnumerable<T> IgnoredMembers => (IEnumerable<T>)this.ignoredMembers ?? EmptyMembers;

        public bool IsIgnoringDeclaringType(Type declaringType)
        {
            return this.ignoredTypes.IsIgnoringType(declaringType);
        }

        protected bool IsIgnoringMember(T instance)
        {
            return this.ignoredMembers?.Contains(instance) == true;
        }
    }
}