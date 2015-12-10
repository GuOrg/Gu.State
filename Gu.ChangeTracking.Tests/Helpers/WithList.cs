using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Gu.ChangeTracking.Tests.Helpers
{
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