namespace Gu.State
{
    using System;

    public interface IGetterAndSetter
    {
        Type SourceType { get; }

        Type ValueType { get; }

        void SetValue(object source, object value);

        object GetValue(object source);

        bool ValueEquals(object x, object y);
    }
}
