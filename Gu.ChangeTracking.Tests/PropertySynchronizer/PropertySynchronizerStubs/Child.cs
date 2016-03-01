namespace Gu.ChangeTracking.Tests.PropertySynchronizerStubs
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class Child : INotifyPropertyChanged
    {
        private string name;

        private Parent parent;

        public Child()
        {
        }

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