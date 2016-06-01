namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal sealed class RootChanges : IDisposable
    {
        private bool disposed;

        private RootChanges(object source, PropertiesSettings settings)
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
                inpc.PropertyChanged += this.OnSourcePropertyChanged;
            }

            var incc = source as INotifyCollectionChanged;
            if (incc != null)
            {
                incc.CollectionChanged += this.OnSourceCollectionChanged;
            }
        }

        public event EventHandler<PropertyChangeEventArgs> PropertyChange;

        public event EventHandler<ResetEventArgs> Reset;

        public event EventHandler<AddEventArgs> Add;

        public event EventHandler<RemoveEventArgs> Remove;

        public event EventHandler<ReplaceEventArgs> Replace;

        public event EventHandler<MoveEventArgs> Move;

        public event EventHandler<EventArgs> RawChange;

        public object Source { get; }

        public IReadOnlyCollection<PropertyInfo> TrackProperties { get; }

        public PropertiesSettings Settings { get; }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            var inpc = this.Source as INotifyPropertyChanged;
            if (inpc != null)
            {
                inpc.PropertyChanged -= this.OnSourcePropertyChanged;
            }

            var incc = this.Source as INotifyCollectionChanged;
            if (incc != null)
            {
                incc.CollectionChanged -= this.OnSourceCollectionChanged;
            }
        }

        internal static IRefCounted<RootChanges> GetOrCreate(INotifyPropertyChanged source, PropertiesSettings settings, bool isRoot)
        {
            return GetOrCreate((object)source, settings, isRoot);
        }

        internal static IRefCounted<RootChanges> GetOrCreate(INotifyCollectionChanged source, PropertiesSettings settings, bool isRoot)
        {
            return GetOrCreate((object)source, settings, isRoot);
        }

        internal static IRefCounted<RootChanges> GetOrCreate(object source, PropertiesSettings settings, bool isRoot)
        {
            Debug.Assert(source != null, "Cannot track null");
            if (isRoot)
            {
                Track.Verify.IsTrackableType(source.GetType(), settings);
            }
            else
            {
                Track.Verify.IsTrackableValue(source, settings);
            }

            return TrackerCache.GetOrAdd(source, settings, s => new RootChanges(s, settings));
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RawChange?.Invoke(this, e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        this.Add?.Invoke(this, new AddEventArgs((IList)sender, e.NewStartingIndex + i));
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        this.Remove?.Invoke(this, new RemoveEventArgs((IList)sender, e.OldStartingIndex + i));
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        this.Replace?.Invoke(this, new ReplaceEventArgs((IList)sender, e.NewStartingIndex + i));
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                    {
                        this.Move?.Invoke(this, new MoveEventArgs((IList)sender, e.OldStartingIndex, e.NewStartingIndex));
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.Reset?.Invoke(this, new ResetEventArgs((IList)sender));
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(e.Action));
            }
        }

        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                this.OnResetProperties(sender, e);
                return;
            }

            var propertyInfo = sender.GetType().GetProperty(e.PropertyName, this.Settings.BindingFlags);

            if (this.Settings.IsIgnoringProperty(propertyInfo))
            {
                return;
            }

            this.RawChange?.Invoke(this, e);
            this.PropertyChange?.Invoke(this, new PropertyChangeEventArgs(sender, propertyInfo));
        }

        private void OnResetProperties(object sender, PropertyChangedEventArgs e)
        {
            this.RawChange?.Invoke(this, e);
            var handler = this.PropertyChange;
            if (handler != null)
            {
                foreach (var propertyInfo in this.Settings.GetProperties(sender.GetType()))
                {
                    if (this.Settings.IsIgnoringProperty(propertyInfo))
                    {
                        continue;
                    }

                    handler.Invoke(this, new PropertyChangeEventArgs(sender, propertyInfo));
                }
            }
        }
    }
}
