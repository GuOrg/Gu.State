namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;

    public interface ISpecialCopyProperty
    {
        PropertyInfo Property { get; }

        Func<object, object> CreateCopyValue { get; }
    }
}