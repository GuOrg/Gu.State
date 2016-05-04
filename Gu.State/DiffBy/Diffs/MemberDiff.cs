namespace Gu.State
{
    using System.Reflection;

    public abstract class MemberDiff : SubDiff
    {
        protected MemberDiff(ValueDiff diff)
            : base(diff)
        {
        }

        internal abstract MemberInfo Member { get;  }

        internal static MemberDiff Create(MemberInfo member, object xValue, object yValue)
        {
            return Create(member, new ValueDiff(xValue, yValue));
        }

        internal static MemberDiff Create(MemberInfo member, ValueDiff diff)
        {
            var fieldInfo = member as FieldInfo;
            if (fieldInfo != null)
            {
                return new FieldDiff(fieldInfo, diff);
            }

            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                return new PropertyDiff(propertyInfo, diff);
            }

            throw State.Throw.ExpectedParameterOfTypes<PropertyInfo, FieldInfo>("MemberDiff.Create(MemberInfo member, object xValue, object yValue)");
        }
    }
}