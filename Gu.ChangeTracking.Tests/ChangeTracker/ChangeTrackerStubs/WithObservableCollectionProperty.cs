namespace Gu.ChangeTracking.Tests.ChangeTrackerStubs
{
    using System.Collections.ObjectModel;

    public class WithObservableCollectionProperty<T> : ComplexType
    {
        private ObservableCollection<T> values = new ObservableCollection<T>();

        public ObservableCollection<T> Values
        {
            get
            {
                return this.values;
            }
            set
            {
                if (Equals(value, this.values))
                {
                    return;
                }
                this.values = value;
                this.OnPropertyChanged();
            }
        }
    }
}