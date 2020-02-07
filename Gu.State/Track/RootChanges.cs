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
            this.TrackProperties = source.GetType()
                                       .GetProperties()
                                       .Where(p => !settings.IsIgnoringProperty(p))
                                       .Where(p => !settings.IsImmutable(p.PropertyType))
                                       .ToArray();
            if (source is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged += this.OnSourcePropertyChanged;
            }

            if (source is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += this.OnSourceCollectionChanged;
            }
        }

        internal event EventHandler<PropertyChangeEventArgs> PropertyChange;

        internal event EventHandler<ResetEventArgs> Reset;

        internal event EventHandler<AddEventArgs> Add;

        internal event EventHandler<RemoveEventArgs> Remove;

        internal event EventHandler<ReplaceEventArgs> Replace;

        internal event EventHandler<MoveEventArgs> Move;

        internal event EventHandler<EventArgs> RawChange;

        internal object Source { get; }

        internal IReadOnlyCollection<PropertyInfo> TrackProperties { get; }

        internal PropertiesSettings Settings { get; }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (this.Source is INotifyPropertyChanged inpc)
            {
                inpc.PropertyChanged -= this.OnSourcePropertyChanged;
            }

            if (this.Source is INotifyCollectionChanged incc)
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
                Track.Verify.CanTrackType(source.GetType(), settings);
            }
            else
            {
                Track.Verify.CanTrackValue(source, settings);
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
                    throw new ArgumentOutOfRangeException(nameof(e), e.Action, "Unknown NotifyCollectionChangedAction.");
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
