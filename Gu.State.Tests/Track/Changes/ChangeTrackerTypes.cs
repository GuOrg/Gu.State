namespace Gu.State.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Gu.State.Tests.EqualByTests;

    using JetBrains.Annotations;

    public static class ChangeTrackerTypes
    {
        public interface IBaseClass
        {
            int Value { get; set; }
            int Excluded { get; set; }
        }

        public class ComplexType : INotifyPropertyChanged, IBaseClass
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
                get { return this.value; }
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
                get { return this.name; }
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
                get { return this.comparison; }
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
                get { return this.next; }
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
                get { return this.ints; }
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
                get { return this.levels; }
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

            [NotifyPropertyChangedInvocator]
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
            public event PropertyChangedEventHandler PropertyChanged;

            public T Value
            {
                get { return this.value; }
                set
                {
                    if (Equals(value, this.value)) return;
                    this.value = value;
                    this.OnPropertyChanged();
                }
            }

            [NotifyPropertyChangedInvocator]
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
                get { return this.value; }
                set
                {
                    if (value == this.value) return;
                    this.value = value;
                    this.OnPropertyChanged();
                }
            }

            public IllegalType Illegal
            {
                get { return this.illegal; }
                set
                {
                    if (Equals(value, this.illegal)) return;
                    this.illegal = value;
                    this.OnPropertyChanged();
                }
            }

            [NotifyPropertyChangedInvocator]
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
            public IllegalType Illegal { get; set; }
        }

        public class IllegalEnumerable : INotifyPropertyChanged, IEnumerable<int>
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public IEnumerator<int> GetEnumerator()
            {
                return Enumerable.Empty<int>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            [NotifyPropertyChangedInvocator]
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
                this.Name = name;
                if (child != null)
                {
                    child.Parent = this;
                }

                this.Child = child;
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

            [NotifyPropertyChangedInvocator]
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
                this.Name = name;
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

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
