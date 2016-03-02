namespace Gu.ChangeTracking
{
    using System;

    public interface IEqualBySettings : IBindingFlags, IReferenceHandling
    {
        bool IsIgnoringDeclaringType(Type declaringType);
    }
}