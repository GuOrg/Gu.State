// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Gu.State.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class ComplexType : INotifyPropertyChanged
    {
        private string name;
        private int value;

        public ComplexType()
        {
        }

        public ComplexType(string name, int value)
        {
            this.name = name;
            this.value = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get => this.name;
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
            get => this.value;
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

        public static bool operator ==(ComplexType left, ComplexType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ComplexType left, ComplexType right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((ComplexType)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Name?.GetHashCode() ?? 0) * 397) ^ this.Value;
            }
        }

        protected bool Equals(ComplexType other) => string.Equals(this.Name, other.Name, StringComparison.Ordinal) &&
                                                    this.Value == other.Value;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}