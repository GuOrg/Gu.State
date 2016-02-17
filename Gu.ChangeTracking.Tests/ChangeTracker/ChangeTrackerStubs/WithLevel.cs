namespace Gu.ChangeTracking.Tests.ChangeTrackerStubs
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    public class WithLevel : INotifyPropertyChanged
    {
        private Level level;
        public event PropertyChangedEventHandler PropertyChanged;

        public Level Level
        {
            get { return this.level; }
            set
            {
                if (Equals(value, this.level)) return;
                this.level = value;
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