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

        bool ValueEquals(object x, object y);

        void CopyValue(object source, object target);
    }
}
