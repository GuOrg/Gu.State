namespace Gu.State.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public static class SynchronizeTypes
    {
        public class With<T> : INotifyPropertyChanged
        {
            private T value;

            private string name;

            public event PropertyChangedEventHandler PropertyChanged;

            public T Value
            {
                get => this.value;
                set
                {
                    if (Equals(value, this.value))
                    {
                        return;
                    }

                    this.value = value;
                    this.OnPropertyChanged();
                }
            }

            public string Name
            {
                get => this.name;
                set
                {
                    if (value == this.name)
                    {
                        return;
                    }

                    this.name = value;
                    this.OnPropertyChanged();
                }
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class ComplexType : INotifyPropertyChanged
        {
            public static readonly TestComparer Comparer = new();

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

            public event PropertyChangedEventHandler PropertyChanged;

            public string Name
            {
                get => this.name;
                set
                {
                    if (value == this.name)
                    {
                        return;
                    }

                    this.name = value;
                    this.OnPropertyChanged();
                }
            }

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
                }
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

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

                    return string.Equals(x.name, y.name, StringComparison.Ordinal) && x.value == y.value;
                }

                public int GetHashCode(ComplexType obj)
                {
                    unchecked
                    {
                        return ((obj.name?.GetHashCode() ?? 0) * 397) ^ obj.value;
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

        public class WithComplexProperty : INotifyPropertyChanged
        {
            private string name;

            private int value;

            private ComplexType complexType;

            public WithComplexProperty()
            {
            }

            public WithComplexProperty(string name, int value)
            {
                this.name = name;
                this.value = value;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public string Name
            {
                get => this.name;
                set
                {
                    if (value == this.name)
                    {
                        return;
                    }

                    this.name = value;
                    this.OnPropertyChanged();
                }
            }

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
                }
            }

            public ComplexType ComplexType
            {
                get => this.complexType;
                set
                {
                    if (Equals(value, this.complexType))
                    {
                        return;
                    }

                    this.complexType = value;
                    this.OnPropertyChanged();
                }
            }

            public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public void SetFields(string name, int value, ComplexType complexType)
            {
                this.name = name;
                this.value = value;
                this.complexType = complexType;
            }
        }

        public sealed class WithGetReadOnlyPropertySealed<T> : IEquatable<WithGetReadOnlyPropertySealed<T>>
        {
            public WithGetReadOnlyPropertySealed(T value)
            {
                this.Value = value;
            }

            public T Value { get; }

            public static bool operator ==(WithGetReadOnlyPropertySealed<T> left, WithGetReadOnlyPropertySealed<T> right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(WithGetReadOnlyPropertySealed<T> left, WithGetReadOnlyPropertySealed<T> right)
            {
                return !Equals(left, right);
            }

            public bool Equals(WithGetReadOnlyPropertySealed<T> other)
            {
                if (other is null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return EqualityComparer<T>.Default.Equals(this.Value, other.Value);
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

                return obj is WithGetReadOnlyPropertySealed<T> && this.Equals((WithGetReadOnlyPropertySealed<T>)obj);
            }

            public override int GetHashCode()
            {
                return EqualityComparer<T>.Default.GetHashCode(this.Value);
            }
        }

        public class WithImmutableProperty : INotifyPropertyChanged
        {
            private string name;
            private int value;
            private WithGetReadOnlyPropertySealed<int> immutableValue;

            public WithImmutableProperty()
            {
            }

            public WithImmutableProperty(string name, int value)
            {
                this.name = name;
                this.value = value;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public string Name
            {
                get => this.name;
                set
                {
                    if (value == this.name)
                    {
                        return;
                    }

                    this.name = value;
                    this.OnPropertyChanged();
                }
            }

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
                }
            }

            public WithGetReadOnlyPropertySealed<int> ImmutableValue
            {
                get => this.immutableValue;
                set
                {
                    if (Equals(value, this.immutableValue))
                    {
                        return;
                    }

                    this.immutableValue = value;
                    this.OnPropertyChanged();
                }
            }

            public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public void SetFields(string name, int value, WithGetReadOnlyPropertySealed<int> immutableValue)
            {
                this.name = name;
                this.value = value;
                this.immutableValue = immutableValue;
            }
        }

        public class WithObservableCollectionProperties : INotifyPropertyChanged
        {
            private string name;
            private int value;

            public WithObservableCollectionProperties()
            {
            }

            public WithObservableCollectionProperties(string name, int value)
            {
                this.name = name;
                this.value = value;
            }

            public WithObservableCollectionProperties(params ComplexType[] complexTypes)
            {
                foreach (var complexType in complexTypes)
                {
                    this.Complexes.Add(complexType);
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public string Name
            {
                get => this.name;
                set
                {
                    if (value == this.name)
                    {
                        return;
                    }

                    this.name = value;
                    this.OnPropertyChanged();
                }
            }

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
                }
            }

            public ObservableCollection<ComplexType> Complexes { get; } = new();

            public ObservableCollection<int> Ints { get; } = new();

            public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class WithSimpleProperties : INotifyPropertyChanged
        {
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
        }

        public class WithTwoComplexProperties : INotifyPropertyChanged
        {
            private string name;

            private int value;

            private ComplexType complexValue1;

            private ComplexType complexValue2;

            public WithTwoComplexProperties()
            {
            }

            public WithTwoComplexProperties(string name, int value)
            {
                this.name = name;
                this.value = value;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public string Name
            {
                get => this.name;
                set
                {
                    if (value == this.name)
                    {
                        return;
                    }

                    this.name = value;
                    this.OnPropertyChanged();
                }
            }

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
                }
            }

            public ComplexType ComplexValue1
            {
                get => this.complexValue1;
                set
                {
                    if (Equals(value, this.complexValue1))
                    {
                        return;
                    }

                    this.complexValue1 = value;
                    this.OnPropertyChanged();
                }
            }

            public ComplexType ComplexValue2
            {
                get => this.complexValue2;
                set
                {
                    if (Equals(value, this.complexValue2))
                    {
                        return;
                    }

                    this.complexValue2 = value;
                    this.OnPropertyChanged();
                }
            }

            public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public void SetFields(string name, int value, ComplexType complexType)
            {
                this.name = name;
                this.value = value;
                this.complexValue1 = complexType;
            }
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

        public class Parent : INotifyPropertyChanged
        {
            private string name;

            private Child child;

            public Parent(string name, Child child)
            {
                this.name = name;
                this.Child = child;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public string Name
            {
                get => this.name;
                set
                {
                    if (value == this.name)
                    {
                        return;
                    }

                    this.name = value;
                    this.OnPropertyChanged();
                }
            }

            public Child Child
            {
                get => this.child;
                set
                {
                    if (Equals(value, this.child))
                    {
                        return;
                    }

                    this.child = value;
                    if (this.child != null)
                    {
                        this.child.Parent = this;
                    }

                    this.OnPropertyChanged();
                }
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class Child : INotifyPropertyChanged
        {
            private string name;

            private Parent parent;

            public Child()
            {
            }

            public Child(string name)
            {
                this.name = name;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public string Name
            {
                get => this.name;
                set
                {
                    if (value == this.name)
                    {
                        return;
                    }

                    this.name = value;
                    this.OnPropertyChanged();
                }
            }

            public Parent Parent
            {
                get => this.parent;
                set
                {
                    if (Equals(value, this.parent))
                    {
                        return;
                    }

                    this.parent = value;
                    this.OnPropertyChanged();
                }
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public abstract class BaseClass : INotifyPropertyChanged
        {
            private double baseValue;

            public event PropertyChangedEventHandler PropertyChanged;

            public double BaseValue
            {
                get => this.baseValue;
                set
                {
                    if (value.Equals(this.baseValue))
                    {
                        return;
                    }

                    this.baseValue = value;
                    this.OnPropertyChanged();
                }
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class Derived1 : BaseClass
        {
            private double derived1Value;

            public double Derived1Value
            {
                get => this.derived1Value;
                set
                {
                    if (value.Equals(this.derived1Value))
                    {
                        return;
                    }

                    this.derived1Value = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public class Derived2 : BaseClass
        {
            private double derived2Value;

            public double Derived2Value
            {
                get => this.derived2Value;
                set
                {
                    if (value.Equals(this.derived2Value))
                    {
                        return;
                    }

                    this.derived2Value = value;
                    this.OnPropertyChanged();
                }
            }
        }
    }
}
