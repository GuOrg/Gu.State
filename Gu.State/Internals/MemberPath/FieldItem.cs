namespace Gu.State
{
    using System;
    using System.Reflection;

    internal class FieldItem : PathItem, IMemberItem
    {
        internal FieldItem(FieldInfo field)
        {
            this.Field = field;
        }

        MemberInfo IMemberItem.Member => this.Field;

        Type ITypedNode.Type => this.Field.FieldType;

        internal FieldInfo Field { get; }
    }
}