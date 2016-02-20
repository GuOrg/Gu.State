namespace Gu.ChangeTracking.Tests.DirtyTrackerStubs
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class WithObservableCollectionProperties : INotifyPropertyChanged
    {
        private string name;
        private int value;

        public WithObservableCollectionProperties()
        {
        }

        public WithObservableCollectionProperties(string name, int value)
        {
            this.Name = name;
            this.Value = value;
        }

        public WithObservableCollectionProperties(params ComplexType[] complexTypes)
        {
            foreach (var complexType in complexTypes)
            {
                this.Complexes.Add(complexType);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value == this.name)
                {
                    return;
                }
                this.name = value;
                this.OnPropertyChanged();
            }
        }

        public int Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (value == this.value)
                {
                    return;
                }
                this.value = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<ComplexType> Complexes { get; } = new ObservableCollection<ComplexType>();

        public ObservableCollection<int> Ints { get; } = new ObservableCollection<int>();

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}