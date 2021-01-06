#pragma warning disable SA1600 // Elements should be documented
namespace Gu.State
{
    using System.Reflection;

    internal interface INotsupportedMember
    {
        MemberInfo Member { get; }
    }
}
