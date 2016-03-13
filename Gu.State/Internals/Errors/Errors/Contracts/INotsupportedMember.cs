namespace Gu.State
{
    using System.Reflection;

    internal interface INotsupportedMember
    {
        MemberInfo Member { get; }
    }
}