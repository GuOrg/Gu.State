// ReSharper disable All
#pragma warning disable INPC003 // Notify when property changes.
#pragma warning disable INPC001 // Implement INotifyPropertyChanged.
namespace Gu.State.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public static class DirtyTrackerTypes
    {
        public struct WithGetReadOnlyPropertyStruct<T>
        {
            public WithGetReadOnlyPropertyStruct(T value)
            {
                this.Value = value;
            }

            public T Value { get; }
        }

        public class With<T> : INotifyPropertyChanged
        {
            private T value;

            private string name;

            public event PropertyChangedEventHandler PropertyChanged;

            public T Value
            {
                get
                {
                    return this.value;
                }

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
                get
                {
                    return this.name;
                }

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
            public static readonly TestComparer Comparer = new TestComparer();
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
                get
                {
                    return this.name;
                }

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
                get
                {
                    return this.value;
                }

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

                    return string.Equals(x.Name, y.Name, StringComparison.Ordinal) && x.Value == y.Value;
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

        public class IllegalType
        {
            public int Value { get; set; }
        }

        public class IllegalSubType : ComplexType
        {
#pragma warning disable INPC002 // Mutable public property should notify.
            public IllegalType Illegal { get; set; }
#pragma warning restore INPC002 // Mutable public property should notify.
        }

        public class WithSimpleProperties : INotifyPropertyChanged
        {
            private int value;
            private DateTime time;

            public WithSimpleProperties()
            {
            }

            public WithSimpleProperties(int value, DateTime time)
            {
                this.value = value;
                this.time = time;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public int Value
            {
                get
                {
                    return this.value;
                }

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

            public DateTime Time
            {
                get
                {
                    return this.time;
                }

                set
                {
                    if (value == this.time)
                    {
                        return;
                    }

                    this.time = value;
                    this.OnPropertyChanged();
                }
            }

            public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public void SetFields(int value, DateTime time)
            {
                this.Value = value;
                this.Time = time;
            }
        }

        public class WithComplexProperty : INotifyPropertyChanged
        {
            private string name;

            private int value;

            private ComplexType complexType;

            public event PropertyChangedEventHandler PropertyChanged;

            public string Name
            {
                get
                {
                    return this.name;
                }

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
                get
                {
                    return this.value;
                }

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
                get
                {
                    return this.complexType;
                }

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

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
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

                    return Enumerable.SequenceEqual(x.ints, y.ints);
                }

                public int GetHashCode(IntCollection obj)
                {
                    throw new NotImplementedException("message");
                }
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
                if (ReferenceEquals(null, other))
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
                if (ReferenceEquals(null, obj))
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

        public class Level : INotifyPropertyChanged
        {
            private int value;
            private Level next;
            private ObservableCollection<int> ints = new ObservableCollection<int>();
            private ObservableCollection<Level> levels = new ObservableCollection<Level>();
            private string name;
            private StringComparison comparison;

            public event PropertyChangedEventHandler PropertyChanged;

            public int Value
            {
                get
                {
                    return this.value;
                }

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

            public string Name
            {
                get
                {
                    return this.name;
                }

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

            public StringComparison Comparison
            {
                get
                {
                    return this.comparison;
                }

                set
                {
                    if (value == this.comparison)
                    {
                        return;
                    }

                    this.comparison = value;
                    this.OnPropertyChanged();
                }
            }

            public Level Next
            {
                get
                {
                    return this.next;
                }

                set
                {
                    if (Equals(value, this.next))
                    {
                        return;
                    }

                    this.next = value;
                    this.OnPropertyChanged();
                }
            }

            public ObservableCollection<int> Ints
            {
                get
                {
                    return this.ints;
                }

                set
                {
                    if (Equals(value, this.ints))
                    {
                        return;
                    }

                    this.ints = value;
                    this.OnPropertyChanged();
                }
            }

            public ObservableCollection<Level> Levels
            {
                get
                {
                    return this.levels;
                }

                set
                {
                    if (Equals(value, this.levels))
                    {
                        return;
                    }

                    this.levels = value;
                    this.OnPropertyChanged();
                }
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class WithImmutableProperty : INotifyPropertyChanged
        {
            private string name;

            private int value;

            private WithGetReadOnlyPropertySealed<int> immutableValue;

            public event PropertyChangedEventHandler PropertyChanged;

            public string Name
            {
                get
                {
                    return this.name;
                }

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
                get
                {
                    return this.value;
                }

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
                get
                {
                    return this.immutableValue;
                }

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

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                get
                {
                    return this.name;
                }

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
                get
                {
                    return this.value;
                }

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

            public ObservableCollection<ComplexType> Complexes { get; } = new ObservableCollection<ComplexType>();

            public ObservableCollection<int> Ints { get; } = new ObservableCollection<int>();

            public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
                get
                {
                    return this.baseValue;
                }

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
                get
                {
                    return this.derived1Value;
                }

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
                get
                {
                    return this.derived2Value;
                }

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