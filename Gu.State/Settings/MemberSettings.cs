namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    public abstract partial class MemberSettings
    {
        private static readonly IReadOnlyDictionary<Type, CastingComparer> DefaultComparers = new Dictionary<Type, CastingComparer>
        {
            { typeof(IntPtr), CastingComparer.Create(EqualityComparer<IntPtr>.Default) },
        };

        private readonly Lazy<ConcurrentDictionary<Type, TypeErrors>> copyErrors = new Lazy<ConcurrentDictionary<Type, TypeErrors>>();
        private readonly ImmutableSet<Type> immutableTypes;
        private readonly ConcurrentDictionary<Type, EqualByComparer> rootEqualByComparers = new ConcurrentDictionary<Type, EqualByComparer>();
        private readonly ConcurrentDictionary<Type, EqualByComparer> equalByComparers = new ConcurrentDictionary<Type, EqualByComparer>();
        private readonly IReadOnlyDictionary<Type, CastingComparer> comparers;
        private readonly IReadOnlyDictionary<Type, CustomCopy> copyers;
        private readonly KnownTypes knownTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberSettings"/> class.
        /// </summary>
        /// <param name="ignoredMembers">A collection of members to ignore. Can be null.</param>
        /// <param name="ignoredTypes">A collection of types to ignore. Can be null.</param>
        /// <param name="immutableTypes">A collection of types to treat as immutable. Can be null.</param>
        /// <param name="comparers">A map of types with custom comparers. Can be null.</param>
        /// <param name="copyers">A map of custom copy implementations for types. Can be null.</param>
        /// <param name="referenceHandling">How reference values are handled.</param>
        /// <param name="bindingFlags">The <see cref="BindingFlags"/> used for getting members.</param>
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
            this.comparers = comparers;
            this.copyers = copyers;
            this.knownTypes = KnownTypes.Create(ignoredTypes);
            this.immutableTypes = ImmutableSet<Type>.Create(immutableTypes);

            if (ignoredMembers != null)
            {
                foreach (var ignoredMember in ignoredMembers)
                {
                    this.IgnoredMembers.TryAdd(ignoredMember, true);
                }
            }

            foreach (var comparer in DefaultComparers.Concat(comparers ?? Enumerable.Empty<KeyValuePair<Type, CastingComparer>>()))
            {
                this.equalByComparers[comparer.Key] = new ExplicitEqualByComparer(comparer.Value);
                this.rootEqualByComparers[comparer.Key] = new ExplicitEqualByComparer(comparer.Value);
            }
        }

        /// <summary>Gets the <see cref="BindingFlags"/> used for getting members.</summary>
        public BindingFlags BindingFlags { get; }

        /// <summary>Gets a value indicating how reference values are handled.</summary>
        public ReferenceHandling ReferenceHandling { get; }

        internal ConcurrentDictionary<Type, TypeErrors> CopyErrors => this.copyErrors.Value;

        /// <summary>Gets a cache for ignored members.</summary>
        protected ConcurrentDictionary<MemberInfo, bool> IgnoredMembers { get; } = new ConcurrentDictionary<MemberInfo, bool>();

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

        /// <summary>Gets all instance members that matches <see cref="BindingFlags"/>.</summary>
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

        /// <summary>Try get a custom comparer for <paramref name="type"/>.</summary>
        /// <param name="type">The type.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>True if a custom comparer is provided for <paramref name="type"/>.</returns>
        public bool TryGetComparer(Type type, out CastingComparer comparer)
        {
            comparer = null;
            return this.comparers?.TryGetValue(type, out comparer) == true;
        }

        /// <summary>Try get a custom copyer for <paramref name="type"/>.</summary>
        /// <param name="type">The type.</param>
        /// <param name="copyer">The copyer.</param>
        /// <returns>True if a custom copyer is provided for <paramref name="type"/>.</returns>
        public bool TryGetCopyer(Type type, out CustomCopy copyer)
        {
            copyer = null;
            return this.copyers?.TryGetValue(type, out copyer) == true;
        }

        internal IEnumerable<MemberInfo> GetEffectiveMembers(Type type)
        {
            return this.GetMembers(type).Where(x => !this.IsIgnoringMember(x) && !x.IsIndexer());
        }

        internal EqualByComparer GetRootEqualByComparer(Type type)
        {
            Debug.Assert(type != null, "type != null");
            return this.rootEqualByComparers.GetOrAdd(type, _ => Create());

            EqualByComparer Create()
            {
                return EqualByComparer.Create(type, this);
            }
        }

        internal EqualByComparer GetEqualByComparer(Type type)
        {
            Debug.Assert(type != null, "type != null");
            return this.equalByComparers.GetOrAdd(type, t => Create());

            EqualByComparer Create()
            {
                if (!type.IsValueType &&
                    !this.IsEquatable(type))
                {
                    switch (this.ReferenceHandling)
                    {
                        case ReferenceHandling.Throw:
                            return new ErrorEqualByComparer(type, new TypeErrors(type, RequiresReferenceHandling.Default));
                        case ReferenceHandling.References:
                            return ReferenceEqualByComparer.Default;
                        case ReferenceHandling.Structural:
                            break;
                    }
                }

                return EqualByComparer.Create(type, this);
            }
        }
    }
}