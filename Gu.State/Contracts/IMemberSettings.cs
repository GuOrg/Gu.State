namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>Setting specifying how members are handled.</summary>
    public interface IMemberSettings
    {
        /// <summary>Gets the bindingflags used for getting members.</summary>
        BindingFlags BindingFlags { get; }

        /// <summary>Gets a value indicating how reference values are handled.</summary>
        ReferenceHandling ReferenceHandling { get; }

        /// <summary>
        /// Check if <paramref name="type"/> is equatable.
        /// There are two ways a type can be equatable:
        /// 1) Implements IEquatable{self}
        /// 2) By explicitly providing a setting that the type is equatable
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if <paramref name="type"/> is equatable</returns>
        bool IsEquatable(Type type);

        /// <summary>
        /// Check if <paramref name="type"/> is immutable.
        /// There are two ways a type can be immutable:
        /// 1) The following holds
        ///   - Is a struct or sealed class.
        ///   - All members are readonly
        ///   - ALl member types are immutable
        /// 2) By explicitly providing a setting that the type is immutable
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if <paramref name="type"/> is equatable</returns>
        bool IsImmutable(Type type);

        /// <summary>Gets all instance members that matches <see cref="BindingFlags"/></summary>
        /// <param name="type">The type to get members for.</param>
        /// <returns>The members.</returns>
        IEnumerable<MemberInfo> GetMembers(Type type);

        /// <summary>Gets if the <paramref name="member"/> is ignored.</summary>
        /// <param name="member">The member to check.</param>
        /// <returns>A value indicating if <paramref name="member"/> is ignored.</returns>
        bool IsIgnoringMember(MemberInfo member);

        /// <summary>Gets if the <paramref name="declaringType"/> is ignored.</summary>
        /// <param name="declaringType">The type to check.</param>
        /// <returns>A value indicating if <paramref name="declaringType"/> is ignored.</returns>
        bool IsIgnoringDeclaringType(Type declaringType);

        /// <summary>Get an <see cref="IGetterAndSetter"/> that is  used for getting ans setting values.</summary>
        /// <param name="member">The member.</param>
        /// <returns>A <see cref="IGetterAndSetter"/></returns>
        IGetterAndSetter GetOrCreateGetterAndSetter(MemberInfo member);

        /// <summary>Try get a custom comparer for <paramref name="type"/></summary>
        /// <param name="type">The type.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>True if a custom comparer is provided for <paramref name="type"/></returns>
        bool TryGetComparer(Type type, out CastingComparer comparer);

        /// <summary>Try get a custom copyer for <paramref name="type"/></summary>
        /// <param name="type">The type.</param>
        /// <param name="copyer">The copyer.</param>
        /// <returns>True if a custom copyer is provided for <paramref name="type"/></returns>
        bool TryGetCopyer(Type type, out CustomCopy copyer);
    }
}