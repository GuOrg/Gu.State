namespace Gu.ChangeTracking.Tests.DirtyTrackerStubs
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

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
}