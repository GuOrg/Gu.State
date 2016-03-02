namespace Gu.ChangeTracking
{
    using System;

    public interface IIgnoringDeclaredType
    {
        bool IsIgnoringDeclaringType(Type declaredType);
    }
}