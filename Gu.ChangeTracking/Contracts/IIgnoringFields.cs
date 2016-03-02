namespace Gu.ChangeTracking
{
    using System.Reflection;

    public interface IIgnoringFields : IBindingFlags
    {
        bool IsIgnoringField(FieldInfo fieldInfo);
    }
}