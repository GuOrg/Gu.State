namespace Gu.ChangeTracking
{
    using System;
    using System.Reflection;

    internal class PropertyItem : PathItem, IMemberItem
    {
        public PropertyItem(PropertyInfo property)
        {
            this.Property = property;
        }

        public PropertyInfo Property { get; }

        MemberInfo IMemberItem.Member => this.Property;

        Type ITypedNode.Type => this.Property.PropertyType;
    }
}