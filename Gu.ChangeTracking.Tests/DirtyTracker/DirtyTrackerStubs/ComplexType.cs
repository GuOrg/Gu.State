namespace Gu.ChangeTracking.Tests.DirtyTrackerStubs
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class ComplexType : INotifyPropertyChanged
    {
        private string name;

        private int value;

        public ComplexType()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ComplexType(string name, int value)
        {
            this.Name = name;
            this.Value = value;
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
    }
}