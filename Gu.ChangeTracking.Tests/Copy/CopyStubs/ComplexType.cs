namespace Gu.ChangeTracking.Tests.CopyStubs
{
    using System.Collections;
    using System.Collections.Generic;

    public class ComplexType
    {
        public static readonly TestComparer Comparer = new TestComparer();

        public ComplexType()
        {
        }

        public ComplexType(string name, int value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }

        public int Value { get; set; }

        public sealed class TestComparer : IEqualityComparer<ComplexType>, IComparer<ComplexType>, IComparer
        {
            public bool Equals(ComplexType x, ComplexType y)
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
                return string.Equals(x.Name, y.Name) && x.Value == y.Value;
            }

            public int GetHashCode(ComplexType obj)
            {
                unchecked
                {
                    return ((obj.Name?.GetHashCode() ?? 0) * 397) ^ obj.Value;
                }
            }

            public int Compare(ComplexType x, ComplexType y)
            {
                return this.Equals(x, y)
                           ? 0
                           : -1;
            }

            int IComparer.Compare(object x, object y)
            {
                return this.Compare((ComplexType)x, (ComplexType)y);
            }
        }
    }
}