namespace Gu.ChangeTracking
{
    using System.Reflection;

    internal class PropertyItem : PathItem
    {
        public PropertyItem(PropertyInfo property)
        {
            this.Property = property;
        }

        public PropertyInfo Property { get; }
    }
}