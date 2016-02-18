namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    internal sealed class ItemSynchronizerCollection : Collection<PropertySynchronizer<INotifyPropertyChanged>>, IDisposable
    {
        public void Dispose()
        {
            this.ClearItems();
        }

        public void Move(int index, int toIndex)
        {
            var synchronizer = this.Items[index];
            this.Items.RemoveAt(index);
            this.Insert(toIndex, synchronizer);
        }

        protected override void InsertItem(int index, PropertySynchronizer<INotifyPropertyChanged> item)
        {
            this.FillTo(index);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, PropertySynchronizer<INotifyPropertyChanged> item)
        {
            this.FillTo(index);
            this.TryGet(index)?.Dispose();
            base.SetItem(index, item);
        }

        protected override void ClearItems()
        {
            foreach (var item in this.Items)
            {
                item?.Dispose();
            }

            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            this.TryGet(index)?.Dispose();
            base.RemoveItem(index);
        }

        private void FillTo(int index)
        {
            while (this.Items.Count < index)
            {
                this.Items.Add(null);
            }
        }

        private PropertySynchronizer<INotifyPropertyChanged> TryGet(int index)
        {
            if (index >= this.Count)
            {
                return null;
            }

            return this[index];
        }
    }
}
