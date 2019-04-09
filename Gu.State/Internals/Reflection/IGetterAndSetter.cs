namespace Gu.State
{
    using System;
    using System.Reflection;

    /// <summary>Provides functionlaity for getting and setting values for a member.</summary>
    internal interface IGetterAndSetter
    {
        /// <summary>Gets the declaring type of the member.</summary>
        Type SourceType { get; }

        /// <summary>Gets the value type of the member.</summary>
        Type ValueType { get; }

        /// <summary>
        /// Gets a value indicating whether the member is init only.
        /// For a field this means private readonly...
        /// For a property this means public int Value { get; }.
        /// </summary>
        bool IsInitOnly { get; }

        /// <summary>Gets the member.</summary>
        MemberInfo Member { get; }

        void SetValue(object source, object value);

        object GetValue(object source);

        /// <summary>
        /// Check if x and y can be equated by value semantics.
        /// This can be if:
        /// 1) either or both are null.
        /// 2) They are the same instance.
        /// 3) They are of types implementing IEquatable
        /// 4) An IEqualityComparer if provided for the type.
        ///  </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="equal">True if <paramref name="x"/> and <paramref name="y"/> are equal.</param>
        /// <param name="xv">The x value fetched for the member.</param>
        /// <param name="yv">The y value fetched for the member.</param>
        /// <returns>True if equality could be determined for <paramref name="x"/> and <paramref name="y"/>.</returns>
        bool TryGetValueEquals(object x, object y, MemberSettings settings, out bool equal, out object xv, out object yv);

        void CopyValue(object source, object target);
    }
}
