namespace Gu.State.Tests.ChangeTrackerStubs
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class WithList : INotifyPropertyChanged
    {
        private List<int> list;
        public event PropertyChangedEventHandler PropertyChanged;

        public List<int> List
        {
            get { return this.list; }
            set
            {
                if (Equals(value, this.list)) return;
                this.list = value;
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