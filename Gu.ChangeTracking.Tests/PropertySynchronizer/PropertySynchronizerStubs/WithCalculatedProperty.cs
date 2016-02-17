namespace Gu.ChangeTracking.Tests.PropertySynchronizerStubs
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class WithCalculatedProperty : INotifyPropertyChanged
    {
        private readonly int factor;
        private int value;

        public WithCalculatedProperty(int factor = 1)
        {
            this.factor = factor;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get { return this.value; }
            set
            {
                if (value == this.value) return;
                this.value = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.CalculatedValue));
            }
        }

        public int CalculatedValue => this.Value * this.factor;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}