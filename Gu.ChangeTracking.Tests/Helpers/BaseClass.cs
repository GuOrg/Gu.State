namespace Gu.ChangeTracking.Tests.Helpers
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    public class BaseClass : INotifyPropertyChanged, IBaseClass
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
                OnPropertyChanged();
            }
        }

        public int Excluded
        {
            get { return this.excluded; }
            set
            {
                if (value == this.excluded) return;
                this.excluded = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetFields(int value, int excluded)
        {
            Value = value;
            Excluded = excluded;
        }
    }
}
