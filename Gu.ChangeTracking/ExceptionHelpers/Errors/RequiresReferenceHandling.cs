namespace Gu.ChangeTracking
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{GetType().Name} Type: {Type}")]
    internal class RequiresReferenceHandling : TypeError
    {
        public RequiresReferenceHandling(Type type)
            : base(type)
        {
        }
    }
}