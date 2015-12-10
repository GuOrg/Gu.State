using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Gu.ChangeTracking.Tests.Helpers
{
    using System.ComponentModel;

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
                OnPropertyChanged();
            }
        }

        public IllegalObject Illegal
        {
            get { return this.illegal; }
            set
            {
                if (Equals(value, this.illegal)) return;
                this.illegal = value;
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
