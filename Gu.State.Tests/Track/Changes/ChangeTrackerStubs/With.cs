namespace Gu.State.Tests.ChangeTrackerStubs
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class With<T> : INotifyPropertyChanged
    {
        private T value;
        public event PropertyChangedEventHandler PropertyChanged;

        public T Value
        {
            get { return this.value; }
            set
            {
                if (Equals(value, this.value)) return;
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