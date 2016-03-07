namespace Gu.State
{
    using System.Reflection;

    public interface IIgnoringProperties
    {
        bool IsIgnoringProperty(PropertyInfo propertyInfo);
    }
}