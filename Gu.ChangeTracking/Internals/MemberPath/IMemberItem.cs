namespace Gu.ChangeTracking
{
    using System.Reflection;

    internal interface IMemberItem : ITypedNode
    {
        MemberInfo Member { get; }
    }
}