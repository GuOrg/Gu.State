namespace Gu.State
{
    using System.Reflection;

    internal class FieldDiff : MemberDiff<FieldInfo>
    {
        public FieldDiff(FieldInfo fieldInfo, object xValue, object yValue)
            : this(fieldInfo, new ValueDiff(xValue, yValue))
        {
        }

        public FieldDiff(FieldInfo fieldInfo, ValueDiff diff)
            : base(fieldInfo, diff)
        {
        }

        public FieldInfo FieldInfo => base.MemberyInfo;
    }
}