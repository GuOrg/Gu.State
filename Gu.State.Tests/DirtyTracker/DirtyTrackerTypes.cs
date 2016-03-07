namespace Gu.State.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public static class DirtyTrackerTypes
    {
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
                this.Name = name;
                this.Value = value;
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

            [NotifyPropertyChangedInvocator]
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

        public class SimpleDirtyTrackClass : INotifyPropertyChanged
        {
            private int value;
            private int excluded;

            public event PropertyChangedEventHandler PropertyChanged;

            public int Value
            {
                get { return this.value; }
                set
                {
                    if (value == this.value) return;
                    this.value = value;
                    this.OnPropertyChanged();
                }
            }

            public int Excluded
            {
                get { return this.excluded; }
                set
                {
                    if (value == this.excluded) return;
                    this.excluded = value;
                    this.OnPropertyChanged();
                }
            }

            [NotifyPropertyChangedInvocator]
            public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public void SetFields(int value, int excluded)
            {
                this.Value = value;
                this.Excluded = excluded;
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

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        public struct WithGetReadOnlyPropertyStruct<T>
        {
            public WithGetReadOnlyPropertyStruct(T value)
            {
                this.Value = value;
            }

            public T Value { get; }
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

            [NotifyPropertyChangedInvocator]
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
                this.Name = name;
                this.Value = value;
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

            [NotifyPropertyChangedInvocator]
            public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}