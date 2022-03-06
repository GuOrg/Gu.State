// ReSharper disable All
namespace Gu.State.Tests.EqualByTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public static class EqualByTypes
    {
        public interface IWith
        {
            object Value { get; }
        }

        public struct Struct
        {
            public int Value { get; set; }
        }

        public struct EquatableStruct : IEquatable<EquatableStruct>
        {
            public int Value { get; set; }

            public static bool operator ==(EquatableStruct left, EquatableStruct right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(EquatableStruct left, EquatableStruct right)
            {
                return !(left == right);
            }

            public override bool Equals(object obj) => obj is EquatableStruct other &&
                                                       this.Equals(other);

            public bool Equals(EquatableStruct other) => this.Value == other.Value;

            public override int GetHashCode() => this.Value;
        }

        public class With<T> : IWith
        {
            public With(T value)
            {
                this.Value = value;
            }

            public T Value { get; }

            object IWith.Value => this.Value;

#pragma warning disable CA1508 // Avoid dead conditional code, broken analyzer
            public override string ToString() => $"With<{typeof(T).PrettyName()}> {{ {this.Value?.ToString() ?? "null"} }}";
#pragma warning restore CA1508 // Avoid dead conditional code
        }

        public class WithGetSet<T> : IWith
        {
            public WithGetSet(T value)
            {
                this.Value = value;
            }

            public T Value { get; set; }

            object IWith.Value => this.Value;

#pragma warning disable CA1508 // Avoid dead conditional code, broken analyzer
            public override string ToString() => $"With<{typeof(T).PrettyName()}> {{ {this.Value?.ToString() ?? "null"} }}";
#pragma warning restore CA1508 // Avoid dead conditional code
        }

        public class IntCollection : IReadOnlyList<int>
        {
            public static readonly IEqualityComparer<IntCollection> Comparer = new IntsEqualityComparer();
            private readonly IReadOnlyList<int> ints;

            public IntCollection(params int[] ints)
            {
                this.ints = ints;
            }

            public int Count => this.ints.Count;

            public int this[int index] => this.ints[index];

            public IEnumerator<int> GetEnumerator() => this.ints.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            private sealed class IntsEqualityComparer : IEqualityComparer<IntCollection>
            {
                public bool Equals(IntCollection x, IntCollection y)
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

                    return Enumerable.SequenceEqual(x.ints, y.ints);
                }

                public int GetHashCode(IntCollection obj)
                {
                    throw new NotSupportedException("message");
                }
            }
        }

        public sealed class EquatableIntCollection : IReadOnlyList<int>, IEquatable<EquatableIntCollection>
        {
            private readonly IReadOnlyList<int> ints;

            public EquatableIntCollection(params int[] ints)
            {
                this.ints = ints;
            }

            public int Count => this.ints.Count;

            public int this[int index] => this.ints[index];

            public IEnumerator<int> GetEnumerator() => this.ints.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            public bool Equals(EquatableIntCollection other)
            {
                if (other is null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return Enumerable.SequenceEqual(this.ints, other.ints);
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

                if (obj.GetType() != typeof(EquatableIntCollection))
                {
                    return false;
                }

                return this.Equals((EquatableIntCollection)obj);
            }

#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
            public override int GetHashCode() => throw new NotSupportedException("message");
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
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
            public static readonly TestComparer Comparer = new();

            public static readonly IEqualityComparer<ComplexType> ByNameComparer = new NameComparer();

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

            public override string ToString() => $"{this.GetType().Name} {{ Name: {this.Name}, Value: {this.Value} }}";

            public sealed class TestComparer : IEqualityComparer<ComplexType>, IComparer<ComplexType>, IComparer
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

                    return string.Equals(x.Name, y.Name, StringComparison.Ordinal)
                           && x.Value == y.Value;
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

            public override bool Equals(object obj) => obj is Immutable immutable && this.Equals(immutable);

            public override int GetHashCode() => this.Value;
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
            public WithComplexProperty()
            {
            }

            public WithComplexProperty(string name, int value)
            {
                this.Name = name;
                this.Value = value;
            }

            public WithComplexProperty(string name, int value, ComplexType complexType)
            {
                this.Name = name;
                this.Value = value;
                this.ComplexType = complexType;
            }

            public string Name { get; set; }

            public int Value { get; set; }

            public ComplexType ComplexType { get; set; }
        }

        public class WithListProperty<T>
        {
            public List<T> Items { get; set; } = new();
        }

        public class WithSimpleProperties : INotifyPropertyChanged
        {
            internal static readonly AllMembersComparerImpl AllMembersComparer = new();

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

            internal sealed class AllMembersComparerImpl : IEqualityComparer<WithSimpleProperties>, IComparer
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

        public class SealedParent
        {
            public SealedParent(string name, SealedChild child)
            {
                this.Name = name;
                if (child != null)
                {
                    child.Parent = this;
                }

                this.Child = child;
            }

            public string Name { get; }

            public SealedChild Child { get; set; }
        }

        public class SealedChild
        {
            public SealedChild(string name)
            {
                this.Name = name;
            }

            public string Name { get; }

            public SealedParent Parent { get; set; }
        }

        public sealed class HashCollisionType
        {
            public int HashValue { get; set; }

            public int Value { get; set; }

            public override bool Equals(object obj)
            {
                return obj is HashCollisionType type &&
                       this.HashValue == type.HashValue &&
                       this.Value == type.Value;
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

            public override string ToString() => $"{this.GetType().Name} {{ BaseValue: {this.BaseValue}, Derived1Value: {this.Derived1Value} }}";
        }

        public class Derived2 : BaseClass
        {
            public double Derived2Value { get; set; }

            public override string ToString() => $"{this.GetType().Name} {{ BaseValue: {this.BaseValue}, Derived2Value: {this.Derived2Value} }}";
        }
    }
}
