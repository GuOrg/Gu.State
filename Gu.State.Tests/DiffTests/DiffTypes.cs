// ReSharper disable All
#pragma warning disable INPC003 // Notify when property changes.
#pragma warning disable INPC001 // Implement INotifyPropertyChanged.
namespace Gu.State.Tests.DiffTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public static class DiffTypes
    {
        public class With<T>
        {
            public With(T value)
            {
                this.Value = value;
            }

            public T Value { get; }
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
                this.name = name;
                this.value = value;
            }

            public string Name
            {
                get => this.name;
                set => this.name = value;
            }

            public int Value
            {
                get => this.value;
                set => this.value = value;
            }

            public ComplexType ComplexValue
            {
                get => this.complexValue;
                set => this.complexValue = value;
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
                this.intValue = intValue;
                this.nullableIntValue = nullableIntValue;
                this.stringValue = stringValue;
                this.enumValue = enumValue;
            }

            public int IntValue
            {
                get => this.intValue;
                set => this.intValue = value;
            }

            public int? NullableIntValue
            {
                get => this.nullableIntValue;
                set => this.nullableIntValue = value;
            }

            public string StringValue
            {
                get => this.stringValue;
                set => this.stringValue = value;
            }

            public StringSplitOptions EnumValue
            {
                get => this.enumValue;
                set => this.enumValue = value;
            }

            public override string ToString()
            {
                return $"({this.IntValue}, {this.NullableIntValue}, {this.StringValue}, {this.EnumValue})";
            }
        }

        public class ComplexType
        {
            public static readonly IEqualityComparer<ComplexType> ByNameComparer = new NameComparer();

            private string name;
            private int value;

            public ComplexType()
            {
            }

            public ComplexType(string name, int value)
            {
                this.name = name;
                this.value = value;
            }

            public string Name
            {
                get => this.name;
                set => this.name = value;
            }

            public int Value
            {
                get => this.value;
                set => this.value = value;
            }

            private sealed class NameComparer : IEqualityComparer<ComplexType>
            {
                public bool Equals(ComplexType x, ComplexType y)
                {
                    if (ReferenceEquals(x, y))
                    {
                        return true;
                    }

                    if (x is null)
                    {
                        return false;
                    }

                    if (y is null)
                    {
                        return false;
                    }

                    if (x.GetType() != y.GetType())
                    {
                        return false;
                    }

                    return string.Equals(x.Name, y.Name, StringComparison.Ordinal);
                }

                public int GetHashCode(ComplexType obj)
                {
                    return obj?.Name.GetHashCode() ?? 0;
                }
            }
        }

        public sealed class Immutable : IEquatable<Immutable>
        {
            public Immutable(int value)
            {
                this.Value = value;
            }

            public int Value { get; }

            public static bool operator ==(Immutable left, Immutable right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Immutable left, Immutable right)
            {
                return !Equals(left, right);
            }

            public bool Equals(Immutable other)
            {
                if (other is null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return this.Value == other.Value;
            }

            public override bool Equals(object obj)
            {
                if (obj is null)
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                return obj is Immutable && this.Equals((Immutable)obj);
            }

            public override int GetHashCode()
            {
                return this.Value;
            }
        }

        public class WithArrayProperty
        {
            public WithArrayProperty()
            {
            }

            public WithArrayProperty(string name, int value)
            {
                this.Name = name;
                this.Value = value;
            }

            public WithArrayProperty(string name, int value, int[] array)
            {
                this.Name = name;
                this.Value = value;
                this.Array = array;
            }

            public string Name { get; set; }

            public int Value { get; set; }

            public int[] Array { get; set; }
        }

        public class WithCalculatedProperty : INotifyPropertyChanged
        {
            private readonly int factor;
            private int value;

            public WithCalculatedProperty(int factor = 1)
            {
                this.factor = factor;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public int Value
            {
                get => this.value;

                set
                {
                    if (value == this.value)
                    {
                        return;
                    }

                    this.value = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(this.CalculatedValue));
                }
            }

            public int CalculatedValue => this.Value * this.factor;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class WithComplexProperty
        {
            private ComplexType complexType;

            private int value;

            private string name;

            public WithComplexProperty()
            {
            }

            public WithComplexProperty(string name, int value)
            {
                this.name = name;
                this.value = value;
            }

            public WithComplexProperty(string name, int value, ComplexType complexType)
            {
                this.name = name;
                this.value = value;
                this.complexType = complexType;
            }

            public string Name
            {
                get => this.name;
                set => this.name = value;
            }

            public int Value
            {
                get => this.value;
                set => this.value = value;
            }

            public ComplexType ComplexType
            {
                get => this.complexType;
                set => this.complexType = value;
            }
        }

        public class WithProperty<T>
        {
            public T Value { get; set; }
        }

        public class WithListProperty<T>
        {
            public List<T> Items { get; set; } = new List<T>();
        }

        public class WithReadonlyProperty<T>
        {
            public WithReadonlyProperty(T value)
            {
                this.Value = value;
            }

            public T Value { get; }
        }

        public class WithSimpleProperties : INotifyPropertyChanged
        {
            internal static readonly TestComparer Comparer = new TestComparer();

            private int intValue;
            private int? nullableIntValue;
            private string stringValue;
            private StringSplitOptions enumValue;

            public WithSimpleProperties()
            {
            }

            public WithSimpleProperties(int intValue, int? nullableIntValue, string stringValue, StringSplitOptions enumValue)
            {
                this.intValue = intValue;
                this.nullableIntValue = nullableIntValue;
                this.stringValue = stringValue;
                this.enumValue = enumValue;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public int IntValue
            {
                get => this.intValue;

                set
                {
                    if (value == this.intValue)
                    {
                        return;
                    }

                    this.intValue = value;
                    this.OnPropertyChanged();
                }
            }

            public int? NullableIntValue
            {
                get => this.nullableIntValue;

                set
                {
                    if (value == this.nullableIntValue)
                    {
                        return;
                    }

                    this.nullableIntValue = value;
                    this.OnPropertyChanged();
                }
            }

            public string StringValue
            {
                get => this.stringValue;

                set
                {
                    if (value == this.stringValue)
                    {
                        return;
                    }

                    this.stringValue = value;
                    this.OnPropertyChanged();
                }
            }

            public StringSplitOptions EnumValue
            {
                get => this.enumValue;

                set
                {
                    if (value == this.enumValue)
                    {
                        return;
                    }

                    this.enumValue = value;
                    this.OnPropertyChanged();
                }
            }

            public void SetFields(int intValue, string stringValue)
            {
                this.intValue = intValue;
                this.stringValue = stringValue;
            }

            public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            internal sealed class TestComparer : IEqualityComparer<WithSimpleProperties>, IComparer
            {
                public bool Equals(WithSimpleProperties x, WithSimpleProperties y)
                {
                    if (ReferenceEquals(x, y))
                    {
                        return true;
                    }

                    if (x is null)
                    {
                        return false;
                    }

                    if (y is null)
                    {
                        return false;
                    }

                    if (x.GetType() != y.GetType())
                    {
                        return false;
                    }

                    return x.IntValue == y.IntValue &&
                           x.NullableIntValue == y.NullableIntValue &&
                           string.Equals(x.StringValue, y.StringValue, StringComparison.Ordinal) &&
                           x.EnumValue == y.EnumValue;
                }

                public int GetHashCode(WithSimpleProperties obj)
                {
                    unchecked
                    {
                        var hashCode = obj.IntValue;
                        hashCode = (hashCode * 397) ^ obj.NullableIntValue.GetHashCode();
                        hashCode = (hashCode * 397) ^ (obj.StringValue?.GetHashCode() ?? 0);
                        hashCode = (hashCode * 397) ^ (int)obj.EnumValue;
                        return hashCode;
                    }
                }

                int IComparer.Compare(object x, object y)
                {
                    return this.Equals((WithSimpleProperties)x, (WithSimpleProperties)y)
                               ? 0
                               : -1;
                }
            }
        }

        public class WithIllegalIndexer
        {
            // ReSharper disable once UnusedParameter.Global
            public int this[int index]
            {
                get { return 0; }
                set { }
            }
        }

        public class Parent
        {
            public Parent(string name, Child child)
            {
                this.Name = name;
                if (child != null)
                {
                    child.Parent = this;
                }

                this.Child = child;
            }

            public string Name { get; }

            public Child Child { get; set; }
        }

        public class Child
        {
            public Child(string name)
            {
                this.Name = name;
            }

            public string Name { get; }

            public Parent Parent { get; set; }
        }

        public sealed class HashCollisionType
        {
            private int value;

            public int HashValue { get; set; }

            public int Value
            {
                get => this.value;
                set => this.value = value;
            }

            public override int GetHashCode()
            {
                return this.HashValue;
            }
        }

        public abstract class BaseClass
        {
            public double BaseValue { get; set; }
        }

        public class Derived1 : BaseClass
        {
            public double Derived1Value { get; set; }
        }

        public class Derived2 : BaseClass
        {
            public double Derived2Value { get; set; }
        }
    }
}
