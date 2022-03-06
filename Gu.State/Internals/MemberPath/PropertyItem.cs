namespace Gu.State
{
    using System;
    using System.Reflection;

    internal class PropertyItem : PathItem, IMemberItem
    {
        internal PropertyItem(PropertyInfo property)
        {
            this.Property = property;
        }

        MemberInfo IMemberItem.Member => this.Property;

        Type ITypedNode.Type => this.Property.PropertyType;

        internal PropertyInfo Property { get; }
    }
}
