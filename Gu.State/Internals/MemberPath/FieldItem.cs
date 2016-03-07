namespace Gu.State
{
    using System;
    using System.Reflection;

    internal class FieldItem : PathItem, IMemberItem
    {
        public FieldItem(FieldInfo field)
        {
            this.Field = field;
        }

        public FieldInfo Field { get; }

        MemberInfo IMemberItem.Member => this.Field;

        Type ITypedNode.Type => this.Field.FieldType;
    }
}