namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal partial class PropertySynchronizer<T>
    {
        private sealed class ItemsSynchronizer : IDisposable
        {
            private readonly IList source;
            private readonly IList target;
            private readonly PropertiesSettings settings;
            private readonly RefCountCollection<ReferencePair, IPropertySynchronizer> references;
            private readonly DisposingList<IDisposable> itemSynchronizers;
            private bool isSynchronizing;

            private ItemsSynchronizer(
                IList source,
                IList target,
                PropertiesSettings settings,
                RefCountCollection<ReferencePair, IPropertySynchronizer> references)
            {
                this.source = source;
                this.target = target;
                this.settings = settings;
                this.references = references;
                ((INotifyCollectionChanged)source).CollectionChanged += this.OnSourceCollectionChanged;
                var targetColChanged = target as INotifyCollectionChanged;
                if (targetColChanged != null)
                {
                    targetColChanged.CollectionChanged += this.OnTargetCollectionChanged;
                }

                if (!source.GetType().GetItemType().IsImmutable())
                {
                    this.itemSynchronizers = new DisposingList<IDisposable>();
                }

                this.ResetItemSynchronizers();
            }

            public void Dispose()
            {
                this.itemSynchronizers?.Dispose();
                ((INotifyCollectionChanged)this.source).CollectionChanged -= this.OnSourceCollectionChanged;
                var targetColChanged = this.target as INotifyCollectionChanged;
                if (targetColChanged != null)
                {
                    targetColChanged.CollectionChanged += this.OnTargetCollectionChanged;
                }
            }

            internal static ItemsSynchronizer Create(
                T source, 
                T target, 
                PropertiesSettings settings,
                RefCountCollection<ReferencePair, IPropertySynchronizer> references)
            {
                if (!(source is IList))
                {
                    if (source is INotifyCollectionChanged)
                    {
                        throw Throw.ShouldNeverGetHereException($"Can only synchronize collections that are {typeof(IList).Name} and {typeof(INotifyCollectionChanged).Name}");
                    }

                    return null;
                }

                if (!(source is INotifyCollectionChanged))
                {
                    throw Throw.ShouldNeverGetHereException($"Can only synchronize collections that are {typeof(IList).Name} and {typeof(INotifyCollectionChanged).Name}");
                }

                if (!(target is IList))
                {
                    throw Throw.ShouldNeverGetHereException($"Can only synchronize collections that are {typeof(IList).Name} and {typeof(INotifyCollectionChanged).Name}");
                }

                return new ItemsSynchronizer((IList)source, (IList)target, settings, references);
            }

            private void OnTargetCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (!this.isSynchronizing)
                {
                    throw new InvalidOperationException("You cannot modify the target collection when you have applied a PropertySynchronizer on it");
                }
            }

            private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                this.isSynchronizing = true;
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            var sv = e.NewItems[i];
                            object tv = null;
                            if (sv != null)
                            {
                                if (Copy.IsCopyableType(sv.GetType()))
                                {
                                    tv = sv;
                                }
                                else
                                {
                                    tv = Copy.CreateInstance(sv, null, this.settings);
                                    Copy.PropertyValues(sv, tv, this.settings);
                                }
                            }

                            var index = e.NewStartingIndex + i;
                            this.target.Insert(index, tv);
                            this.UpdateItemSynchronizer(index, NotifyCollectionChangedAction.Add);
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            var index = e.OldStartingIndex + i;
                            this.target.RemoveAt(index);
                            this.UpdateItemSynchronizer(index, NotifyCollectionChangedAction.Remove);
                        }

                        break;
                    case NotifyCollectionChangedAction.Replace:
                        this.target[e.NewStartingIndex] = this.source[e.NewStartingIndex];
                        this.UpdateItemSynchronizer(e.NewStartingIndex, NotifyCollectionChangedAction.Replace);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        this.target.GetType().GetMethod("Move", new[] { typeof(int), typeof(int) }).Invoke(this.target, new object[] { e.OldStartingIndex, e.NewStartingIndex });
                        this.UpdateItemSynchronizer(e.OldStartingIndex, NotifyCollectionChangedAction.Move, e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Copy.PropertyValues(this.source, this.target, this.settings);
                        this.ResetItemSynchronizers();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.isSynchronizing = false;
            }

            private void ResetItemSynchronizers()
            {
                if (this.itemSynchronizers == null)
                {
                    return;
                }

                for (int i = 0; i < this.source.Count; i++)
                {
                    this.UpdateItemSynchronizer(i, NotifyCollectionChangedAction.Reset);
                }
            }

            private void UpdateItemSynchronizer(int index, NotifyCollectionChangedAction action, int toIndex = -1)
            {
                if (this.itemSynchronizers == null)
                {
                    return;
                }

                switch (action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            var synchronizer = this.CreateItemSynchronizer(index);
                            this.itemSynchronizers.Insert(index, synchronizer);
                            break;
                        }

                    case NotifyCollectionChangedAction.Remove:
                        this.itemSynchronizers.RemoveAt(index);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        {
                            var synchronizer = this.CreateItemSynchronizer(index);
                            this.itemSynchronizers[index] = synchronizer;
                            break;
                        }

                    case NotifyCollectionChangedAction.Move:
                        this.itemSynchronizers.Move(index, toIndex);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        {
                            var synchronizer = this.CreateItemSynchronizer(index);
                            this.itemSynchronizers[index] = synchronizer;
                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(action), action, null);
                }
            }

            private IDisposable CreateItemSynchronizer(int index)
            {
                var sv = this.source[index];
                if (sv == null)
                {
                    return null;
                }

                if (Copy.IsCopyableType(sv.GetType()))
                {
                    throw new InvalidOperationException("Should not create synchronizers for copy types");
                }

                if (!(this.target.Count > index))
                {
                    throw new InvalidOperationException("Update target before creating synchronizer");
                }

                var tv = this.target[index];
                if (!(sv is INotifyPropertyChanged) || !(tv is INotifyPropertyChanged))
                {
                    throw new NotSupportedException("Can only synchronize items that are INotifyPropertyChanged");
                }

                if (ReferenceEquals(sv, tv))
                {
                    return null;
                }

                if (this.references != null)
                {
                    return this.references.GetOrAdd(
                         new ReferencePair(sv, tv), 
                         () =>
                         new PropertySynchronizer<INotifyPropertyChanged>(
                             (INotifyPropertyChanged)sv,
                             (INotifyPropertyChanged)tv,
                             this.settings,
                             this.references));
                }

                return new PropertySynchronizer<INotifyPropertyChanged>((INotifyPropertyChanged)sv, (INotifyPropertyChanged)tv, this.settings, null);
            }
        }
    }
}
