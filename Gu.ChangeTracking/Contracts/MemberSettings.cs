namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class MemberSettings<T> : IMemberSettings
        where T : MemberInfo
    {
        private static readonly T[] EmptyMembers = new T[0];
        private readonly IgnoredTypes ignoredTypes;
        private readonly HashSet<T> ignoredMembers;

        protected MemberSettings(IEnumerable<T> members, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            this.BindingFlags = bindingFlags;
            this.ReferenceHandling = referenceHandling;
            this.ignoredMembers = members != null
                                      ? new HashSet<T>(members)
                                      : null;
            this.ignoredTypes = IgnoredTypes.Create(null);
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