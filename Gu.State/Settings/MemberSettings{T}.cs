namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>Baseclass for member settings.</summary>
    /// <typeparam name="T">The type, FieldInfo or PropertyInfo</typeparam>
    public abstract class MemberSettings<T> : MemberSettings
        where T : MemberInfo
    {
        private readonly IReadOnlyDictionary<Type, CastingComparer> comparers;
        private readonly IReadOnlyDictionary<Type, CustomCopy> copyers;
        private readonly ImmutableSet<Type> immutableTypes;
        private readonly IgnoredTypes ignoredTypes;
        private readonly ConcurrentDictionary<T, bool> ignoredMembers = new ConcurrentDictionary<T, bool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberSettings{T}"/> class.
        /// </summary>
        /// <param name="ignoredMembers">A collection of members to ignore. Can be null.</param>
        /// <param name="ignoredTypes">A collection of types to ignore. Can be null.</param>
        /// <param name="immutableTypes">A collection of types to treat as immutable. Can be null.</param>
        /// <param name="comparers">A map of types with custom comparers. Can be null.</param>
        /// <param name="copyers">A map of custom copy implementations for types. Can be null.</param>
        /// <param name="referenceHandling">How reference values are handled.</param>
        /// <param name="bindingFlags">The bindingflags used for getting members.</param>
        protected MemberSettings(
            IEnumerable<T> ignoredMembers,
            IEnumerable<Type> ignoredTypes,
            IEnumerable<Type> immutableTypes,
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
            this.immutableTypes = immutableTypes != null
                                      ? new ImmutableSet<Type>(immutableTypes)
                                      : ImmutableSet<Type>.Empty;
        }

        /// <summary>Gets the bindingflags used for getting members.</summary>
        public BindingFlags BindingFlags { get; }

        /// <summary>Gets a value indicating how reference values are handled.</summary>
        public ReferenceHandling ReferenceHandling { get; }

        internal ConcurrentDictionary<Type, TypeErrors> EqualByErrors { get; } = new ConcurrentDictionary<Type, TypeErrors>();

        internal ConcurrentDictionary<Type, TypeErrors> CopyErrors { get; } = new ConcurrentDictionary<Type, TypeErrors>();

        /// <summary>Gets a cache for ignored members.</summary>
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
        public bool IsImmutable(Type type)
        {
            if (this.immutableTypes.Contains(type))
            {
                return true;
            }

            return IsImmutableCore(type);
        }

        /// <summary>Gets if the <paramref name="declaringType"/> is ignored.</summary>
        /// <param name="declaringType">The type to check.</param>
        /// <returns>A value indicating if <paramref name="declaringType"/> is ignored.</returns>
        public bool IsIgnoringDeclaringType(Type declaringType)
        {
            return this.ignoredTypes.IsIgnoringType(declaringType);
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
    }
}