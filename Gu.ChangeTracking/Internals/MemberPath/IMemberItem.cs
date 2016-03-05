namespace Gu.ChangeTracking
{
    using System.Reflection;

    internal interface IMemberItem
    {
        MemberInfo Member { get; }
    }
}