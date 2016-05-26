namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class MemberSettings<T> : MemberSettings, IBindingFlags, IReferenceHandling
        where T : MemberInfo
    {
        private readonly IReadOnlyDictionary<Type, CastingComparer> comparers;
        private readonly IReadOnlyDictionary<Type, CustomCopy> copyers;
        private readonly IgnoredTypes ignoredTypes;
        private readonly ConcurrentDictionary<T, bool> ignoredMembers = new ConcurrentDictionary<T, bool>();

        protected MemberSettings(
            IEnumerable<T> ignoredMembers,
            IEnumerable<Type> ignoredTypes,
            IReadOnlyDictionary<Type, CastingComparer> comparers,
            IReadOnlyDictionary<Type, CustomCopy> copyers,
            ReferenceHandling referenceHandling,
            BindingFlags bindingFlags)
        {
            this.comparers = comparers;
            this.copyers = copyers;
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

        /// <summary>
        /// Check if <paramref name="type"/> is equatable.
        /// There are two ways a type can be equatable:
        /// 1) Implements IEquatable{self}.
        /// 2) By explicitly providing a setting that the type is equatable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if <paramref name="type"/> is equatable.</returns>
        public bool IsEquatable(Type type) => IsEquatableCore(type);

        /// <summary>
        /// Check if <paramref name="type"/> is immutable.
        /// There are two ways a type can be immutable:
        /// 1) The following holds
        ///   - Is a struct or sealed class.
        ///   - All members are readonly
        ///   - All member types are immutable
        /// 2) By explicitly providing a setting that the type is immutable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if <paramref name="type"/> is immutable.</returns>
        public bool IsImmutable(Type type) => IsImmutableCore(type);

        public bool IsIgnoringDeclaringType(Type declaringType)
        {
            return this.ignoredTypes.IsIgnoringType(declaringType);
        }

        public bool TryGetComparer(Type type, out CastingComparer comparer)
        {
            comparer = null;
            return this.comparers?.TryGetValue(type, out comparer) == true;
        }

        public bool TryGetCopyer(Type type, out CustomCopy copyer)
        {
            copyer = null;
            return this.copyers?.TryGetValue(type, out copyer) == true;
        }
    }
}