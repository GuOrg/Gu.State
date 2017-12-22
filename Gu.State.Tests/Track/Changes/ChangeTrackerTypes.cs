// ReSharper disable All
#pragma warning disable INPC003 // Notify when property changes.
#pragma warning disable INPC001 // Implement INotifyPropertyChanged.
#pragma warning disable SA1401 // Fields must be private
namespace Gu.State.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public static class ChangeTrackerTypes
    {
        public interface IBaseClass
        {
            int Value { get; set; }

            int Excluded { get; set; }
        }

#pragma warning disable INPC008 // Struct must not implement INotifyPropertyChanged
        public struct NotifyingStruct : INotifyPropertyChanged
#pragma warning restore INPC008 // Struct must not implement INotifyPropertyChanged
        {
            private int value;

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

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class WithSimpleProperties : INotifyPropertyChanged
        {
            private int value1;
            private DateTime time;

            public event PropertyChangedEventHandler PropertyChanged;

            public int Value1
            {
                get
                {
                    return this.value1;
                }

                set
                {
                    if (value == this.value1)
                    {
                        return;
                    }

                    this.value1 = value;
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
                this.Value1 = value;
                this.Time = time;
            }
        }

        public class ComplexType : INotifyPropertyChanged, IBaseClass
        {
            private int value;
            private int excluded;

            public ComplexType()
            {
            }

            public ComplexType(int value, int excluded)
            {
                this.value = value;
                this.excluded = excluded;
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

            public int Excluded
            {
                get
                {
                    return this.excluded;
                }

                set
                {
                    if (value == this.excluded)
                    {
                        return;
                    }

                    this.excluded = value;
                    this.OnPropertyChanged();
                }
            }

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

        public class DerivedClass : ComplexType
        {
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

        public sealed class Immutable
        {
            public readonly int Value;

            public Immutable()
            {
            }

            public Immutable(int value)
            {
                this.Value = value;
            }
        }

        public class SpecialCollection : ObservableCollection<Level>
        {
        }

        public class WithIllegal : INotifyPropertyChanged
        {
            private int value;
            private IllegalType illegal;

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

            public IllegalType Illegal
            {
                get
                {
                    return this.illegal;
                }

                set
                {
                    if (Equals(value, this.illegal))
                    {
                        return;
                    }

                    this.illegal = value;
                    this.OnPropertyChanged();
                }
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        public class IllegalEnumerable : INotifyPropertyChanged, IEnumerable<int>
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public IEnumerator<int> GetEnumerator()
            {
                return Enumerable.Empty<int>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class Parent : INotifyPropertyChanged
        {
            private Child child;

            private string name;

            public Parent(string name, Child child)
            {
                this.name = name;
                if (child != null)
                {
                    child.Parent = this;
                }

                this.child = child;
            }

            public Parent()
            {
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

            public Child Child
            {
                get
                {
                    return this.child;
                }

                set
                {
                    if (Equals(value, this.child))
                    {
                        return;
                    }

                    this.child = value;
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
            private Parent parent;

            private string name;

            public Child(string name)
            {
                this.name = name;
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

            public Parent Parent
            {
                get
                {
                    return this.parent;
                }

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

        public class WithSelf : INotifyPropertyChanged
        {
            private string name;

            private WithSelf value;

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

            public WithSelf Value
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

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
