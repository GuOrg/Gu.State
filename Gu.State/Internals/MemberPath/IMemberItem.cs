namespace Gu.State
{
    using System.Reflection;

    internal interface IMemberItem : ITypedNode
    {
        MemberInfo Member { get; }
    }
}