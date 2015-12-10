namespace Gu.ChangeTracking.Tests.Helpers
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