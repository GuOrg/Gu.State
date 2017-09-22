namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract partial class MemberSettings
    {
        private readonly Lazy<ConcurrentDictionary<Type, TypeErrors>> equalByErrors = new Lazy<ConcurrentDictionary<Type, TypeErrors>>();
        private readonly Lazy<ConcurrentDictionary<Type, TypeErrors>> copyErrors = new Lazy<ConcurrentDictionary<Type, TypeErrors>>();
        private readonly ImmutableSet<Type> immutableTypes;
        private readonly IReadOnlyDictionary<Type, CastingComparer> comparers;
        private readonly IReadOnlyDictionary<Type, CustomCopy> copyers;
        private readonly KnownTypes knownTypes;
        private readonly ConcurrentDictionary<MemberInfo, bool> ignoredMembers = new ConcurrentDictionary<MemberInfo, bool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberSettings"/> class.
        /// </summary>
        /// <param name="ignoredMembers">A collection of members to ignore. Can be null.</param>
        /// <param name="ignoredTypes">A collection of types to ignore. Can be null.</param>
        /// <param name="immutableTypes">A collection of types to treat as immutable. Can be null.</param>
        /// <param name="comparers">A map of types with custom comparers. Can be null.</param>
        /// <param name="copyers">A map of custom copy implementations for types. Can be null.</param>
        /// <param name="referenceHandling">How reference values are handled.</param>
        /// <param name="bindingFlags">The bindingflags used for getting members.</param>
        protected MemberSettings(
            IEnumerable<MemberInfo> ignoredMembers,
            IEnumerable<Type> ignoredTypes,
            IEnumerable<Type> immutableTypes,
            IReadOnlyDictionary<Type, CastingComparer> comparers,
            IReadOnlyDictionary<Type, CustomCopy> copyers,
            ReferenceHandling referenceHandling,
                        BindingFlags bindingFlags)
        {
            this.ReferenceHandling = referenceHandling;
            this.BindingFlags = bindingFlags;
            if (ignoredMembers != null)
            {
                foreach (var ignoredMember in ignoredMembers)
                {
                    this.ignoredMembers.TryAdd(ignoredMember, true);
                }
            }

            this.knownTypes = KnownTypes.Create(ignoredTypes);
            this.immutableTypes = ImmutableSet<Type>.Create(immutableTypes);
            this.comparers = comparers;
            this.copyers = copyers;
        }

        /// <summary>Gets the bindingflags used for getting members.</summary>
        public BindingFlags BindingFlags { get; }

        /// <summary>Gets a value indicating how reference values are handled.</summary>
        public ReferenceHandling ReferenceHandling { get; }

        internal ConcurrentDictionary<Type, TypeErrors> EqualByErrors => this.equalByErrors.Value;

        internal ConcurrentDictionary<Type, TypeErrors> CopyErrors => this.copyErrors.Value;

        /// <summary>Gets a cache for ignored members.</summary>
        protected ConcurrentDictionary<MemberInfo, bool> IgnoredMembers => this.ignoredMembers;

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
        public bool IsImmutable(Type type)
        {
            if (this.immutableTypes.Contains(type))
            {
                return true;
            }

            return IsImmutableCore(type);
        }

        /// <summary>Gets all instance members that matches <see cref="BindingFlags"/></summary>
        /// <param name="type">The type to get members for.</param>
        /// <returns>The members.</returns>
        public abstract IEnumerable<MemberInfo> GetMembers(Type type);

        /// <summary>Gets if the <paramref name="member"/> is ignored.</summary>
        /// <param name="member">The member to check.</param>
        /// <returns>A value indicating if <paramref name="member"/> is ignored.</returns>
        public abstract bool IsIgnoringMember(MemberInfo member);

        /// <summary>Gets if the <paramref name="declaringType"/> is ignored.</summary>
        /// <param name="declaringType">The type to check.</param>
        /// <returns>A value indicating if <paramref name="declaringType"/> is ignored.</returns>
        public bool IsIgnoringDeclaringType(Type declaringType)
        {
            return this.knownTypes.IsKnownType(declaringType);
        }

        /// <summary>Try get a custom comparer for <paramref name="type"/></summary>
        /// <param name="type">The type.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>True if a custom comparer is provided for <paramref name="type"/></returns>
        public bool TryGetComparer(Type type, out CastingComparer comparer)
        {
            comparer = null;
            return this.comparers?.TryGetValue(type, out comparer) == true;
        }

        /// <summary>Try get a custom copyer for <paramref name="type"/></summary>
        /// <param name="type">The type.</param>
        /// <param name="copyer">The copyer.</param>
        /// <returns>True if a custom copyer is provided for <paramref name="type"/></returns>
        public bool TryGetCopyer(Type type, out CustomCopy copyer)
        {
            copyer = null;
            return this.copyers?.TryGetValue(type, out copyer) == true;
        }

        /// <summary>Get an <see cref="IGetterAndSetter"/> that is  used for getting ans setting values.</summary>
        /// <param name="member">The member.</param>
        /// <returns>A <see cref="IGetterAndSetter"/></returns>
        internal abstract IGetterAndSetter GetOrCreateGetterAndSetter(MemberInfo member);
    }
}