namespace Gu.State
{
    using System;

    /// <summary>Setting specifying how members are handled.</summary>
    public interface IMemberSettings : IIgnoringDeclaredType, IBindingFlags, IReferenceHandling
    {
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
    }
}