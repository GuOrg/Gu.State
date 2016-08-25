namespace Gu.State.Tests
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;
    using NUnit.Framework;

    public partial class DirtyTrackerTests
    {
        public class WithInheritance
        {
            [Test]
            public void BugTest()
            {
                var containerOrg = new ContainerClass();
                var container = new ContainerClass();
                var tracker = Track.IsDirty(containerOrg, container);
                container.SelectedType = new Derived2();
                Assert.IsTrue(tracker.IsDirty);
            }
        }

        public class ContainerClass : INotifyPropertyChanged
        {
            private BaseClass selectedType;

            public BaseClass SelectedType   
            {
                get { return this.selectedType; }
                set
                {
                    if (Equals(value, this.selectedType)) return;
                    this.selectedType = value;
                    this.OnPropertyChanged();
                }
            }

            public ContainerClass()
            {
                this.SelectedType = new Derived1();
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public abstract class BaseClass : INotifyPropertyChanged
        {
            private double baseDouble;

            public double BaseValue
            {
                get { return this.baseDouble; }
                set
                {
                    if (value.Equals(this.baseDouble)) return;
                    this.baseDouble = value;
                    this.OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
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
                get { return this.derived1Value; }
                set
                {
                    if (value.Equals(this.derived1Value)) return;
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
                get { return this.derived2Value; }
                set
                {
                    if (value.Equals(this.derived2Value)) return;
                    this.derived2Value = value;
                    this.OnPropertyChanged();
                }
            }
        }
    }
}