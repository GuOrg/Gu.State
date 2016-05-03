namespace Gu.State
{
    using System;
    using System.Reflection;

    public interface IGetterAndSetter
    {
        Type SourceType { get; }

        Type ValueType { get; }

        bool IsInitOnly { get; }

        MemberInfo Member { get; }

        void SetValue(object source, object value);

        object GetValue(object source);

        /// <summary> </summary>
        /// <param name="equal">True if <paramref name="x"/> and <paramref name="y"/> are equal.</param>
        /// <returns>True if equality could be determined for <paramref name="x"/> and <paramref name="y"/></returns>
        bool TryGetValueEquals(object x, object y, IMemberSettings settings, out bool equal, out object xv, out object yv);

        void CopyValue(object source, object target);
    }
}
