namespace Gu.State
{
    using System.Reflection;

    internal interface IExcludableMember
    {
        MemberInfo Member { get; }
    }
}