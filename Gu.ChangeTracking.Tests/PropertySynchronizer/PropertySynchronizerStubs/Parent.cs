namespace Gu.ChangeTracking.Tests.PropertySynchronizerStubs
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class Parent : INotifyPropertyChanged
    {
        private string name;

        private Child child;

        public Parent(string name, Child child)
        {
            this.Name = name;
            this.Child = child;
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
                if (this.child != null)
                {
                    this.child.Parent = this;
                }
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