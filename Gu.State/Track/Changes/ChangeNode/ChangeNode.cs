namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal sealed class ChangeNode : IDisposable
    {
        private bool disposed;

        private ChangeNode(object source, PropertiesSettings settings)
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

        public event EventHandler<ReplaceEventArgs> Replace;

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

        internal static IRefCounted<ChangeNode> GetOrCreate(object source, PropertiesSettings settings, bool isRoot)
        {
            Debug.Assert(source != null, "Cannot track null");
            Debug.Assert(source is INotifyPropertyChanged || source is INotifyCollectionChanged, "Must notify");
            if (isRoot)
            {
                Track.Verify.IsTrackableType(source.GetType(), settings);
            }
            else
            {
                Track.Verify.IsTrackableValue(source, settings);
            }

            return TrackerCache.GetOrAdd(source, settings, s => new ChangeNode(s, settings));
        }

        private void OnTrackedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Change?.Invoke(this, EventArgs.Empty);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        this.Add?.Invoke(this, new AddEventArgs(e.NewStartingIndex + i));
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        this.Remove?.Invoke(this, new RemoveEventArgs(e.OldStartingIndex + i));
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        this.Replace?.Invoke(this, new ReplaceEventArgs(e.NewStartingIndex + i));
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                    {
                        this.Move?.Invoke(this, new MoveEventArgs(e.OldStartingIndex, e.NewStartingIndex));
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.Reset?.Invoke(this, new ResetEventArgs(e.OldItems, e.NewItems));
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
                this.Change?.Invoke(this, EventArgs.Empty);
                this.OnResetProperties();
                return;
            }

            var propertyInfo = sender.GetType().GetProperty(e.PropertyName, this.Settings.BindingFlags);

            if (this.Settings.IsIgnoringProperty(propertyInfo))
            {
                return;
            }

            this.Change?.Invoke(this, EventArgs.Empty);
            this.PropertyChange?.Invoke(this, new PropertyChangeEventArgs(propertyInfo));
        }

        private void OnResetProperties()
        {
            var handler = this.PropertyChange;
            if (handler != null)
            {
                foreach (var propertyInfo in this.TrackProperties)
                {
                    handler.Invoke(this, new PropertyChangeEventArgs(propertyInfo));
                }
            }
        }
    }
}
