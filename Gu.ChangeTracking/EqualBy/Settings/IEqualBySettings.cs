namespace Gu.ChangeTracking
{
    using System.Reflection;

    public interface IEqualBySettings
    {
        BindingFlags BindingFlags { get; }
        ReferenceHandling ReferenceHandling { get; }
    }
}