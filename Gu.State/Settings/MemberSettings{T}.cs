namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class MemberSettings<T> : MemberSettings, IMemberSettings
        where T : MemberInfo
    {
        private readonly IReadOnlyDictionary<Type, CastingComparer> comparers;
        private readonly IgnoredTypes ignoredTypes;
        private readonly ConcurrentDictionary<T, bool> ignoredMembers = new ConcurrentDictionary<T, bool>();

        protected MemberSettings(
            IEnumerable<T> ignoredMembers,
            IEnumerable<Type> ignoredTypes,
            IReadOnlyDictionary<Type, CastingComparer> comparers,
            BindingFlags bindingFlags,
            ReferenceHandling referenceHandling)
        {
            this.comparers = comparers;
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

        /// <inheritdoc />
        public BindingFlags BindingFlags { get; }

        /// <inheritdoc />
        public ReferenceHandling ReferenceHandling { get; }

        internal ConcurrentDictionary<Type, TypeErrors> EqualByErrors { get; } = new ConcurrentDictionary<Type, TypeErrors>();

        internal ConcurrentDictionary<Type, TypeErrors> CopyErrors { get; } = new ConcurrentDictionary<Type, TypeErrors>();

        protected ConcurrentDictionary<T, bool> IgnoredMembers => this.ignoredMembers;

        /// <inheritdoc />
        public bool IsEquatable(Type type) => IsEquatableCore(type);

        /// <inheritdoc />
        public bool IsImmutable(Type type) => IsImmutableCore(type);

        public abstract IEnumerable<MemberInfo> GetMembers(Type type);

        public abstract bool IsIgnoringMember(MemberInfo member);

        public abstract IGetterAndSetter GetOrCreateGetterAndSetter(MemberInfo member);

        public bool IsIgnoringDeclaringType(Type declaringType)
        {
            return this.ignoredTypes.IsIgnoringType(declaringType);
        }

        public bool TryGetComparer(Type type, out CastingComparer comparer)
        {
            comparer = null;
            return this.comparers?.TryGetValue(type, out comparer) == true;
        }

        internal abstract IGetterAndSetter GetOrCreateGetterAndSetter(T propertyInfo);
    }
}