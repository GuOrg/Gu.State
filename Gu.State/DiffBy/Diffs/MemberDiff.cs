namespace Gu.State
{
    using System.Reflection;

    /// <summary>Base class for a member diff.</summary>
    public abstract class MemberDiff : SubDiff
    {
        /// <summary>Initializes a new instance of the <see cref="MemberDiff"/> class.</summary>
        /// <param name="memberInfo">The member.</param>
        /// <param name="diff">The diff.</param>
        protected MemberDiff(MemberInfo memberInfo, ValueDiff diff)
            : base(diff)
        {
            this.MemberInfo = memberInfo;
        }

        internal MemberInfo MemberInfo { get; }

        internal static MemberDiff Create(MemberInfo member, object xValue, object yValue)
        {
            return Create(member, new ValueDiff(xValue, yValue));
        }

        internal static MemberDiff Create(MemberInfo member, ValueDiff diff)
        {
            if (member is FieldInfo fieldInfo)
            {
                return new FieldDiff(fieldInfo, diff);
            }

            if (member is PropertyInfo propertyInfo)
            {
                return new PropertyDiff(propertyInfo, diff);
            }

            throw Throw.ExpectedParameterOfTypes<PropertyInfo, FieldInfo>("MemberDiff.Create(MemberInfo member, object xValue, object yValue)");
        }
    }
}
