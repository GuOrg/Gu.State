namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;

    public interface IEqualBySettings
    {
        BindingFlags BindingFlags { get; }

        ReferenceHandling ReferenceHandling { get; }

        bool IsIgnoringType(Type type);
    }
}