namespace Gu.ChangeTracking.Tests.DirtyTrackerStubs
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class SimpleDirtyTrackClass : INotifyPropertyChanged
    {
        private int value;
        private int excluded;

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

        public int Excluded
        {
            get { return this.excluded; }
            set
            {
                if (value == this.excluded) return;
                this.excluded = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetFields(int value, int excluded)
        {
            this.Value = value;
            this.Excluded = excluded;
        }
    }
}
