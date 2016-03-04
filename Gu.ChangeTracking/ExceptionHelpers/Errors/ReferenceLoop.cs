namespace Gu.ChangeTracking
{
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("{GetType().Name} Indexer: {Path.PathString}")]
    internal class ReferenceLoop : MemberError
    {
        public ReferenceLoop(MemberInfo member, MemberPath path)
            : base(member, path)
        {
        }
    }
}