namespace Gu.ChangeTracking.Tests.Helpers
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    public class WithIgnoredProperty : INotifyPropertyChanged
    {
        private int value;
        private int ignored;
        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get { return this.value; }
            set
            {
                if (value == this.value) return;
                this.value = value;
                OnPropertyChanged();
            }
        }

        [IgnoreChanges]
        public int Ignored
        {
            get { return this.ignored; }
            set
            {
                if (value == this.ignored) return;
                this.ignored = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
