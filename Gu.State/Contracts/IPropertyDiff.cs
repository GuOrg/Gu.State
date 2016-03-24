namespace Gu.State
{
    using System.Reflection;

    public interface IPropertyDiff : IDiff
    {
        PropertyInfo PropertyInfo { get; }
    }
}