namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal sealed class ChangeTrackerNode : IRefCountable
    {
        private bool disposed;

        private ChangeTrackerNode(object source, PropertiesSettings settings)
        {
            this.Source = source;
            this.Settings = settings;
            this.TrackProperties = this.Source.GetType()
                                       .GetProperties()
                                       .Where(p => !this.Settings.IsIgnoringProperty(p))
                                       .Where(p => !settings.IsImmutable(p.PropertyType))
                                       .ToArray();

            var inpc = source as INotifyPropertyChanged;
            if (inpc != null)
            {
                inpc.PropertyChanged += this.OnTrackedPropertyChanged;
            }

            var incc = source as INotifyCollectionChanged;
            if (incc != null)
            {
                incc.CollectionChanged += this.OnTrackedCollectionChanged;
            }
        }

        public event EventHandler<PropertyChangeEventArgs> PropertyChange;

        public event EventHandler<ResetEventArgs> Reset;

        public event EventHandler<AddEventArgs> Add;

        public event EventHandler<RemoveEventArgs> Remove;

        public event EventHandler<MoveEventArgs> Move;

        public event EventHandler<EventArgs> Change;

        public object Source { get; }

        public IReadOnlyCollection<PropertyInfo> TrackProperties { get; }

        public PropertiesSettings Settings { get; }

        void IDisposable.Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            var inpc = this.Source as INotifyPropertyChanged;
            if (inpc != null)
            {
                inpc.PropertyChanged -= this.OnTrackedPropertyChanged;
            }

            var incc = this.Source as INotifyCollectionChanged;
            if (incc != null)
            {
                incc.CollectionChanged -= this.OnTrackedCollectionChanged;
            }
        }

        internal static IRefCounted<ChangeTrackerNode> GetOrCreate<TOwner>(TOwner owner, object source, PropertiesSettings settings)
        {
            Debug.Assert(source != null, "Cannot track null");
            Debug.Assert(source is INotifyPropertyChanged || source is INotifyCollectionChanged, "Must notify");
            Track.Verify.IsTrackableValue(source, settings);
            return settings.Trackers.GetOrAdd(owner, source, () => new ChangeTrackerNode(source, settings));
        }

        private void OnTrackedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnChange();
            var itemType = sender.GetType().GetItemType();
            if (this.Settings.IsImmutable(itemType))
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        this.OnAdd(new AddEventArgs(e.NewStartingIndex + i, e.NewItems[i]));
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        this.OnRemove(new RemoveEventArgs(e.OldStartingIndex + i, e.OldItems[i]));
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.OnAdd(new AddEventArgs(e.NewStartingIndex, e.NewItems[0]));
                    this.OnRemove(new RemoveEventArgs(e.OldStartingIndex, e.OldItems[0]));
                    break;
                case NotifyCollectionChangedAction.Move:
                    this.OnMove(new MoveEventArgs(e.OldStartingIndex, e.NewStartingIndex));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.OnReset(new ResetEventArgs(e.OldItems, e.NewItems));
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(e.Action));
            }
        }

        private void OnTrackedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                this.OnChange();
                this.OnResetProperties();
                return;
            }

            var propertyInfo = sender.GetType().GetProperty(e.PropertyName, this.Settings.BindingFlags);

            if (this.Settings.IsIgnoringProperty(propertyInfo))
            {
                return;
            }

            this.OnChange();
            this.OnPropertyChange(new PropertyChangeEventArgs(propertyInfo));
        }

        private void OnResetProperties()
        {
            var handler = this.PropertyChange;
            if (handler != null)
            {
                foreach (var propertyInfo in this.TrackProperties)
                {
                    var trackEventArgs = new PropertyChangeEventArgs(propertyInfo);
                    handler.Invoke(this, trackEventArgs);
                }
            }
        }

        private void OnChange()
        {
            this.Change?.Invoke(this, EventArgs.Empty);
        }

        private void OnPropertyChange(PropertyChangeEventArgs e)
        {
            this.PropertyChange?.Invoke(this, e);
        }

        private void OnReset(ResetEventArgs e)
        {
            this.Reset?.Invoke(this, e);
        }

        private void OnAdd(AddEventArgs e)
        {
            this.Add?.Invoke(this, e);
        }

        private void OnRemove(RemoveEventArgs e)
        {
            this.Remove?.Invoke(this, e);
        }

        private void OnMove(MoveEventArgs e)
        {
            this.Move?.Invoke(this, e);
        }
    }
}
