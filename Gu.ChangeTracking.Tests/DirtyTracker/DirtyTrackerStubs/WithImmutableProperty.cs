namespace Gu.ChangeTracking.Tests.DirtyTrackerStubs
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class WithImmutableProperty : INotifyPropertyChanged
    {
        private string name;

        private int value;

        private WithGetReadOnlyPropertySealed<int> immutableValue;

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

        public WithGetReadOnlyPropertySealed<int> ImmutableValue
        {
            get
            {
                return this.immutableValue;
            }
            set
            {
                if (Equals(value, this.immutableValue))
                {
                    return;
                }

                this.immutableValue = value;
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