// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Gu.State.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class ComplexType : INotifyPropertyChanged
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
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(ComplexType))
            {
                return false;
            }

            return this.Equals((ComplexType)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Name?.GetHashCode(StringComparison.Ordinal) ?? 0) * 397) ^ this.Value;
            }
        }

        private bool Equals(ComplexType other) => string.Equals(this.Name, other.Name, StringComparison.Ordinal) &&
                                                    this.Value == other.Value;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
