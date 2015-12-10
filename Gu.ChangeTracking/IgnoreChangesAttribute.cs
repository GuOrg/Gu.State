namespace Gu.ChangeTracking
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property)]
    public class IgnoreChangesAttribute : Attribute
    {
    }
}
