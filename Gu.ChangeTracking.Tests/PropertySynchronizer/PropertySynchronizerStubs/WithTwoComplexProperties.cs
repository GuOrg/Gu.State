namespace Gu.ChangeTracking.Tests.PropertySynchronizerStubs
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class WithTwoComplexProperties : INotifyPropertyChanged
    {
        private string name;

        private int value;

        private ComplexType complexValue1;

        private ComplexType complexValue2;

        public WithTwoComplexProperties()
        {
        }

        public WithTwoComplexProperties(string name, int value)
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

        public ComplexType ComplexValue1
        {
            get
            {
                return this.complexValue1;
            }
            set
            {
                if (Equals(value, this.complexValue1))
                {
                    return;
                }
                this.complexValue1 = value;
                this.OnPropertyChanged();
            }
        }

        public ComplexType ComplexValue2
        {
            get
            {
                return this.complexValue2;
            }
            set
            {
                if (Equals(value, this.complexValue2))
                {
                    return;
                }
                this.complexValue2 = value;
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
            this.complexValue1 = complexType;
        }
    }
}