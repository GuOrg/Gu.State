namespace Gu.ChangeTracking.Tests.ChangeTrackerStubs
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class WithImmutable : INotifyPropertyChanged
    {
        private Immutable immutable;

        public event PropertyChangedEventHandler PropertyChanged;

        public Immutable Immutable
        {
            get
            {
                return this.immutable;
            }
            set
            {
                if (Equals(value, this.immutable))
                {
                    return;
                }
                this.immutable = value;
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