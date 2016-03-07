namespace Gu.State
{
    using System;

    public interface IIgnoringDeclaredType
    {
        bool IsIgnoringDeclaringType(Type declaredType);
    }
}