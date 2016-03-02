namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;

    public interface IMemberSettings : IIgnoringDeclaredType, IBindingFlags, IReferenceHandling
    {
        BindingFlags BindingFlags { get; }

        ReferenceHandling ReferenceHandling { get; }

        bool IsIgnoringDeclaringType(Type declaringType);
    }
}