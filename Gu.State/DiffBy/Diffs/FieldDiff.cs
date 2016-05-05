namespace Gu.State
{
    using System.Reflection;

    /// <summary>A diff for a field.</summary>
    public class FieldDiff : MemberDiff<FieldInfo>
    {
        /// <summary> Initializes a new instance of the <see cref="FieldDiff"/> class.</summary>
        /// <param name="fieldInfo">The field.</param>
        /// <param name="xValue">The x value of the <paramref name="fieldInfo"/></param>
        /// <param name="yValue">The y value of the <paramref name="fieldInfo"/></param>
        public FieldDiff(FieldInfo fieldInfo, object xValue, object yValue)
            : this(fieldInfo, new ValueDiff(xValue, yValue))
        {
        }

        /// <summary> Initializes a new instance of the <see cref="FieldDiff"/> class.</summary>
        /// <param name="fieldInfo">The field.</param>
        /// <param name="diff">The <see cref="ValueDiff"/> for the <paramref name="fieldInfo"/></param>
        public FieldDiff(FieldInfo fieldInfo, ValueDiff diff)
            : base(fieldInfo, diff)
        {
        }

        /// <summary>Gets the field</summary>
        public FieldInfo FieldInfo => this.MemberInfo;
    }
}