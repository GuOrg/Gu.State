namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public abstract class MemberSettings<T> : IMemberSettings
        where T : MemberInfo
    {
        private static readonly T[] EmptyMembers = new T[0];
        private readonly IgnoredTypes ignoredTypes;
        private readonly ConcurrentDictionary<T, bool> ignoredMembers = new ConcurrentDictionary<T, bool>();

        protected MemberSettings(
            IEnumerable<T> ignoredMembers,
            IEnumerable<Type> ignoredTypes,
            BindingFlags bindingFlags,
            ReferenceHandling referenceHandling)
        {
            this.BindingFlags = bindingFlags;
            this.ReferenceHandling = referenceHandling;
            if (ignoredMembers != null)
            {
                foreach (var ignoredMember in ignoredMembers)
                {
                    this.ignoredMembers.TryAdd(ignoredMember, true);
                }
            }

            this.ignoredTypes = IgnoredTypes.Create(ignoredTypes);
        }

        public BindingFlags BindingFlags { get; }

        public ReferenceHandling ReferenceHandling { get; }

        protected ConcurrentDictionary<T, bool> IgnoredMembers => this.ignoredMembers;

        public bool IsIgnoringDeclaringType(Type declaringType)
        {
            return this.ignoredTypes.IsIgnoringType(declaringType);
        }
    }
}