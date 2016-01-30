namespace Gu.ChangeTracking.Tests.ChangeTrackerStubs
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    public class WithIllegalObject : INotifyPropertyChanged
    {
        private int value;
        private IllegalObject illegal;
        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get { return this.value; }
            set
            {
                if (value == this.value) return;
                this.value = value;
                this.OnPropertyChanged();
            }
        }

        public IllegalObject Illegal
        {
            get { return this.illegal; }
            set
            {
                if (Equals(value, this.illegal)) return;
                this.illegal = value;
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
