namespace Gu.State.Benchmarks
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class With<T> : INotifyPropertyChanged
    {
        private string name;

        private T value;

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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}