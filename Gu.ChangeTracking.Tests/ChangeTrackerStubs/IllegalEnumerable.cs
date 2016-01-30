namespace Gu.ChangeTracking.Tests.ChangeTrackerStubs
{
    using System.Collections;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    public class IllegalEnumerable : INotifyPropertyChanged, IEnumerable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerator GetEnumerator()
        {
            return Enumerable.Empty<int>().GetEnumerator();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}