namespace Gu.ChangeTracking
{
    using System;

    public interface IEqualBySettings : IBindingFlags, IReferenceHandling
    {
        bool IsIgnoringType(Type type);
    }
}