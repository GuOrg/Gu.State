namespace Gu.State
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    internal sealed class ChangeTracker : IDisposable
    {
        private readonly object source;
        private readonly PropertiesSettings settings;
        private PropertyInfo[] resetProperties;

        private ChangeTracker(object source, PropertiesSettings settings)
        {
            this.source = source;
            this.settings = settings;
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

        public event EventHandler<UpdateEventArgs> ChildUpdate;

        public event EventHandler<ResetEventArgs> ChildReset;

        public event EventHandler<AddEventArgs> ChildAdd;

        public event EventHandler<RemoveEventArgs> ChildRemove;

        public event EventHandler<MoveEventArgs> ChildMove;

        public event EventHandler<EventArgs> Change;

        public void Dispose()
        {
            var inpc = this.source as INotifyPropertyChanged;
            if (inpc != null)
            {
                inpc.PropertyChanged -= this.OnTrackedPropertyChanged;
            }

            var incc = this.source as INotifyCollectionChanged;
            if (incc != null)
            {
                incc.CollectionChanged -= this.OnTrackedCollectionChanged;
            }
        }

        private void OnTrackedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnChange();
            if (sender.GetType()
                      .GetItemType()
                      .IsImmutable())
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        this.OnChildAdd(new AddEventArgs(e.NewStartingIndex + i, e.NewItems[i]));
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        this.OnChildRemove(new RemoveEventArgs(e.OldStartingIndex + i, e.OldItems[i]));
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.OnChildAdd(new AddEventArgs(e.NewStartingIndex, e.NewItems[0]));
                    this.OnChildRemove(new RemoveEventArgs(e.OldStartingIndex, e.OldItems[0]));
                    break;
                case NotifyCollectionChangedAction.Move:
                    this.OnChildMove(new MoveEventArgs(e.OldStartingIndex, e.NewStartingIndex));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.OnChildReset(new ResetEventArgs(e.OldItems, e.NewItems));
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

            var propertyInfo = sender.GetType().GetProperty(e.PropertyName, this.settings.BindingFlags);

            if (this.settings.IsIgnoringProperty(propertyInfo))
            {
                return;
            }

            if (propertyInfo.PropertyType.IsImmutable())
            {
                this.OnChange();
                return;
            }

            this.OnChange();
            this.OnChildUpdate(new UpdateEventArgs(propertyInfo, propertyInfo.GetValue(this.source)));
        }

        private void OnResetProperties()
        {
            var handler = this.ChildUpdate;
            if (handler != null)
            {
                if (this.resetProperties == null)
                {
                    this.resetProperties = this.source.GetType()
                                               .GetProperties()
                                               .Where(p => !this.settings.IsIgnoringProperty(p))
                                               .Where(p => !p.PropertyType.IsImmutable())
                                               .ToArray();
                }

                foreach (var propertyInfo in this.resetProperties)
                {
                    var trackEventArgs = new UpdateEventArgs(propertyInfo, propertyInfo.GetValue(this.source));
                    handler.Invoke(this, trackEventArgs);
                }
            }
        }

        private void OnChange()
        {
            this.Change?.Invoke(this, EventArgs.Empty);
        }

        private void OnChildUpdate(UpdateEventArgs e)
        {
            this.ChildUpdate?.Invoke(this, e);
        }

        private void OnChildReset(ResetEventArgs e)
        {
            this.ChildReset?.Invoke(this, e);
        }

        private void OnChildAdd(AddEventArgs e)
        {
            this.ChildAdd?.Invoke(this, e);
        }

        private void OnChildRemove(RemoveEventArgs e)
        {
            this.ChildRemove?.Invoke(this, e);
        }

        private void OnChildMove(MoveEventArgs e)
        {
            this.ChildMove?.Invoke(this, e);
        }
    }
}
