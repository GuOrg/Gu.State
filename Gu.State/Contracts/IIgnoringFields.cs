namespace Gu.State
{
    using System.Reflection;

    public interface IIgnoringFields : IBindingFlags
    {
        bool IsIgnoringField(FieldInfo fieldInfo);
    }
}