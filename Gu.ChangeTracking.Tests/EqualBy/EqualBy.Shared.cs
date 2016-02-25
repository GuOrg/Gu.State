namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public partial class EqualByTests
    {
        public static IReadOnlyList<EqualsData> EqualsSource = new List<EqualsData>
        {
            new EqualsData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           true),
            new EqualsData(new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           true),
            new EqualsData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleValues(5, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleValues(1, 5, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleValues(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleValues(1, 2, "5", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new WithSimpleValues(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleValues(1, 2, "3", StringSplitOptions.None),
                           false),
        };

        public class EqualsData
        {
            public EqualsData(object source, object target, bool @equals)
            {
                this.Source = source;
                this.Target = target;
                this.Equals = @equals;
            }

            public object Source { get; }

            public object Target { get; }

            public bool Equals { get; }

            public override string ToString()
            {
                return $"Source: {this.Source}, Target: {this.Target}, Equals: {this.Equals}";
            }
        }

        public class WithComplexValue
        {
            private string name;
            private int value;
            private ComplexType complexValue;

            public WithComplexValue()
            {
            }

            public WithComplexValue(string name, int value)
            {
                this.Name = name;
                this.Value = value;
            }

            public string Name
            {
                get { return this.name; }
                set { this.name = value; }
            }

            public int Value
            {
                get { return this.value; }
                set { this.value = value; }
            }

            public ComplexType ComplexValue
            {
                get { return this.complexValue; }
                set { this.complexValue = value; }
            }
        }

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

        public class WithSimpleValues
        {
            private int intValue;
            private int? nullableIntValue;
            private string stringValue;
            private StringSplitOptions enumValue;

            public WithSimpleValues(int intValue, int? nullableIntValue, string stringValue, StringSplitOptions enumValue)
            {
                this.IntValue = intValue;
                this.NullableIntValue = nullableIntValue;
                this.StringValue = stringValue;
                this.EnumValue = enumValue;
            }

            public int IntValue
            {
                get { return this.intValue; }
                set { this.intValue = value; }
            }

            public int? NullableIntValue
            {
                get { return this.nullableIntValue; }
                set { this.nullableIntValue = value; }
            }

            public string StringValue
            {
                get { return this.stringValue; }
                set { this.stringValue = value; }
            }

            public StringSplitOptions EnumValue
            {
                get { return this.enumValue; }
                set { this.enumValue = value; }
            }

            public override string ToString()
            {
                return $"({this.IntValue}, {this.NullableIntValue}, {this.StringValue}, {this.EnumValue})";
            }
        }
    }
}
