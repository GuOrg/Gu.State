namespace Gu.ChangeTracking.Tests.PropertySynchronizerStubs
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

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
}