namespace Gu.ChangeTracking.Tests.CopyStubs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class WithSimpleFields
    {
        internal static readonly TestComparer Comparer = new TestComparer();

        public WithSimpleFields()
        {
        }

        public WithSimpleFields(int intValue, int? nullableIntValue, string stringValue, StringSplitOptions enumValue)
        {
            this.IntValue = intValue;
            this.NullableIntValue = nullableIntValue;
            this.StringValue = stringValue;
            this.EnumValue = enumValue;
        }

        internal int IntValue;
        internal int? NullableIntValue;
        internal string StringValue;
        internal StringSplitOptions EnumValue;

        internal sealed class TestComparer : IEqualityComparer<WithSimpleFields>, IComparer
        {
            public bool Equals(WithSimpleFields x, WithSimpleFields y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }
                if (ReferenceEquals(x, null))
                {
                    return false;
                }
                if (ReferenceEquals(y, null))
                {
                    return false;
                }
                if (x.GetType() != y.GetType())
                {
                    return false;
                }
                return x.IntValue == y.IntValue && x.NullableIntValue == y.NullableIntValue && string.Equals(x.StringValue, y.StringValue) && x.EnumValue == y.EnumValue;
            }

            public int GetHashCode(WithSimpleFields obj)
            {
                unchecked
                {
                    var hashCode = obj.IntValue;
                    hashCode = (hashCode * 397) ^ obj.NullableIntValue.GetHashCode();
                    hashCode = (hashCode * 397) ^ (obj.StringValue != null
                                                       ? obj.StringValue.GetHashCode()
                                                       : 0);
                    hashCode = (hashCode * 397) ^ (int)obj.EnumValue;
                    return hashCode;
                }
            }

            int IComparer.Compare(object x, object y)
            {
                return this.Equals((WithSimpleFields)x, (WithSimpleFields)y)
                           ? 0
                           : -1;
            }
        }
    }
}
