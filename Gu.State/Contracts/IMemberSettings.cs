using System;

namespace Gu.State
{
    public interface IMemberSettings : IIgnoringDeclaredType, IBindingFlags, IReferenceHandling
    {
        bool IsEquatable(Type type);

        bool IsImmutable(Type type);
    }
}