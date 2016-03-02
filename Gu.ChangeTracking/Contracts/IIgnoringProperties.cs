namespace Gu.ChangeTracking
{
    using System.Reflection;

    public interface IIgnoringProperties
    {
        bool IsIgnoringProperty(PropertyInfo propertyInfo);
    }
}