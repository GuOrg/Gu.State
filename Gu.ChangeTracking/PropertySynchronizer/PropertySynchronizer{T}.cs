namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>
    /// This class tracks property changes in source and keeps target in sync
    /// </summary>
    public class PropertySynchronizer<T> : IDisposable
        where T : class, INotifyPropertyChanged
    {
        private readonly PropertiesSynchronizer propertiesSynchronizer;
        private readonly ItemsSynchronizer itemsSynchronizer;

        public PropertySynchronizer(T source, T target, ReferenceHandling referenceHandling)
            : this(source, target, Constants.DefaultPropertyBindingFlags, referenceHandling)
        {
        }

        public PropertySynchronizer(T source, T target, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : this(source, target, CopyPropertiesSettings.GetOrCreate(bindingFlags, referenceHandling))
        {
        }

        public PropertySynchronizer(T source, T target, params string[] ignoreProperties)
            : this(source, target, Constants.DefaultPropertyBindingFlags, ignoreProperties)
        {
        }

        public PropertySynchronizer(T source, T target, BindingFlags bindingFlags, params string[] ignoreProperties)
            : this(source, target, new CopyPropertiesSettings(source?.GetType(), ignoreProperties, bindingFlags, ReferenceHandling.Throw))
        {
        }

        public PropertySynchronizer(T source, T target, CopyPropertiesSettings settings)
        {
            Ensure.NotSame(source, target, nameof(source), nameof(target));
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Ensure.SameType(source, target, nameof(source), nameof(target));
            Copy.VerifyCanCopyPropertyValues(source?.GetType() ?? typeof(T), settings);
            this.Settings = settings;
            Copy.PropertyValues(source, target, settings);
            this.propertiesSynchronizer = PropertiesSynchronizer.Create(source, target, settings);
            this.itemsSynchronizer = ItemsSynchronizer.Create(source, target, settings);
        }

        public CopyPropertiesSettings Settings { get; }

        public void Dispose()
        {
            this.propertiesSynchronizer?.Dispose();
            this.itemsSynchronizer?.Dispose();
        }

        private class PropertiesSynchronizer : IDisposable
        {
            private readonly INotifyPropertyChanged source;
            private readonly INotifyPropertyChanged target;
            private readonly CopyPropertiesSettings settings;
            private readonly PropertyCollection propertySynchronizers;

            private PropertiesSynchronizer(INotifyPropertyChanged source, INotifyPropertyChanged target, PropertyCollection propertySynchronizers, CopyPropertiesSettings settings)
            {
                source.PropertyChanged += this.OnSourcePropertyChanged;
                this.source = source;
                this.target = target;
                this.settings = settings;
                this.propertySynchronizers = propertySynchronizers;
            }

            public void Dispose()
            {
                this.source.PropertyChanged -= this.OnSourcePropertyChanged;
                this.propertySynchronizers?.Dispose();
            }

            internal static PropertiesSynchronizer Create(INotifyPropertyChanged source, INotifyPropertyChanged target, CopyPropertiesSettings settings)
            {
                List<PropertyCollection.PropertyAndDisposable> items = null;
                foreach (var propertyInfo in source.GetType()
                                                   .GetProperties(settings.BindingFlags))
                {
                    if (settings.IsIgnoringProperty(propertyInfo) || settings.GetSpecialCopyProperty(propertyInfo) != null)
                    {
                        continue;
                    }

                    if (!Copy.IsCopyableType(propertyInfo.PropertyType))
                    {
                        var sv = propertyInfo.GetValue(source);
                        var tv = propertyInfo.GetValue(target);
                        var synchronizer = CreateSynchronizer((INotifyPropertyChanged)sv, (INotifyPropertyChanged)tv, settings);
                        if (items == null)
                        {
                            items = new List<PropertyCollection.PropertyAndDisposable>();
                        }

                        items.Add(new PropertyCollection.PropertyAndDisposable(propertyInfo, synchronizer));
                    }
                }

                if (items == null)
                {
                    return new PropertiesSynchronizer(source, target, null, settings);
                }

                var propertyCollection = new PropertyCollection(items);
                return new PropertiesSynchronizer(source, target, propertyCollection, settings);
            }

            private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (string.IsNullOrEmpty(e.PropertyName))
                {
                    Copy.PropertyValues(this.source, this.target, this.settings);
                    return;
                }

                var propertyInfo = this.source.GetType().GetProperty(e.PropertyName, this.settings.BindingFlags);
                if (propertyInfo == null)
                {
                    return;
                }

                if (this.settings.IsIgnoringProperty(propertyInfo) ||
                    (this.source is INotifyCollectionChanged && e.PropertyName == "Count"))
                {
                    return;
                }

                var specialCopyProperty = this.settings.GetSpecialCopyProperty(propertyInfo);
                if (specialCopyProperty != null)
                {
                    specialCopyProperty.CopyValue(this.source, this.target);
                    return;
                }

                if (!Copy.IsCopyableType(propertyInfo.PropertyType))
                {
                    Copy.PropertyValues(this.source, this.target, this.settings);
                    this.UpdateSubPropertySynchronizer(propertyInfo);
                }
                else
                {
                    Copy.PropertyValue(this.source, this.target, propertyInfo);
                }
            }

            private void UpdateSubPropertySynchronizer(PropertyInfo propertyInfo)
            {
                var sv = (INotifyPropertyChanged)propertyInfo.GetValue(this.source);
                var tv = (INotifyPropertyChanged)propertyInfo.GetValue(this.target);
                this.propertySynchronizers[propertyInfo] = CreateSynchronizer(sv, tv, this.settings);
            }

            private static PropertySynchronizer<INotifyPropertyChanged> CreateSynchronizer(object sv, object tv, CopyPropertiesSettings settings)
            {
                if (sv == null || Copy.IsCopyableType(sv.GetType()))
                {
                    return null;
                }

                if (settings.ReferenceHandling == ReferenceHandling.Throw)
                {
                    throw new NotSupportedException("Specify how to handle reference types using ReferenceHandling");
                }

                if (ReferenceEquals(sv, tv))
                {
                    return null;
                }

                return new PropertySynchronizer<INotifyPropertyChanged>((INotifyPropertyChanged)sv, (INotifyPropertyChanged)tv, settings);
            }
        }

        private class ItemsSynchronizer : IDisposable
        {
            private readonly IList source;
            private readonly IList target;
            private readonly CopyPropertiesSettings settings;
            private readonly ItemCollection<PropertySynchronizer<INotifyPropertyChanged>> itemSynchronizers;

            private bool isSynchronizing;

            public ItemsSynchronizer(IList source, IList target, CopyPropertiesSettings settings)
            {
                this.source = source;
                this.target = target;
                this.settings = settings;
                ((INotifyCollectionChanged)source).CollectionChanged += this.OnSourceCollectionChanged;
                var targetColChanged = target as INotifyCollectionChanged;
                if (targetColChanged != null)
                {
                    targetColChanged.CollectionChanged += this.OnTargetCollectionChanged;
                }

                if (!Copy.IsCopyableType(source.GetType().GetItemType()))
                {
                    this.itemSynchronizers = new ItemCollection<PropertySynchronizer<INotifyPropertyChanged>>();
                }

                this.ResetItemSynchronizers();
            }

            private void OnTargetCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (!this.isSynchronizing)
                {
                    throw new InvalidOperationException("You cannot modify the target collection when you have applied a PropertySynchronizer on it");
                }
            }

            public void Dispose()
            {
                this.itemSynchronizers?.Dispose();
                ((INotifyCollectionChanged)this.source).CollectionChanged -= this.OnSourceCollectionChanged;
                var targetColChanged = this.target as INotifyCollectionChanged;
                if (targetColChanged != null)
                {
                    targetColChanged.CollectionChanged += OnTargetCollectionChanged;
                }
            }

            internal static ItemsSynchronizer Create(T source, T target, CopyPropertiesSettings settings)
            {
                const string canOnlySynchronizeCollectionsThatAreIlistAndInotifycollectionchanged = "Can only synchronize collections that are IList and INotifyCollectionChanged";
                if (!(source is IList))
                {
                    if (source is INotifyCollectionChanged)
                    {
                        throw new NotSupportedException(canOnlySynchronizeCollectionsThatAreIlistAndInotifycollectionchanged);
                    }

                    return null;
                }

                if (!(source is INotifyCollectionChanged))
                {
                    throw new NotSupportedException(canOnlySynchronizeCollectionsThatAreIlistAndInotifycollectionchanged);
                }

                if (!(target is IList))
                {
                    throw new NotSupportedException(canOnlySynchronizeCollectionsThatAreIlistAndInotifycollectionchanged);
                }

                return new ItemsSynchronizer((IList)source, (IList)target, settings);
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
                                    tv = Activator.CreateInstance(sv.GetType(), true);
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

            private PropertySynchronizer<INotifyPropertyChanged> CreateItemSynchronizer(int index)
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

                return new PropertySynchronizer<INotifyPropertyChanged>((INotifyPropertyChanged)sv, (INotifyPropertyChanged)tv, this.settings);
            }
        }
    }
}