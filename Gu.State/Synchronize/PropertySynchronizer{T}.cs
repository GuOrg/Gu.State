namespace Gu.State
{
    using System.ComponentModel;

    /// <summary>
    /// This class tracks property changes in source and keeps target in sync
    /// </summary>
    internal sealed partial class PropertySynchronizer<T> : IPropertySynchronizer
        where T : class, INotifyPropertyChanged
    {
        private readonly PropertiesSynchronizer propertiesSynchronizer;
        private readonly ItemsSynchronizer itemsSynchronizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertySynchronizer{T}"/> class.
        /// </summary>
        /// <typeparam name="T">The type to get ignore properties for settings for</typeparam>
        /// <param name="source">The instance to copy property values from</param>
        /// <param name="target">The instance to copy property values to</param>
        /// <param name="settings">Contains configuration for how synchronization will be performed</param>
        /// <returns>A disposable that when disposed stops synchronizing</returns>
        public PropertySynchronizer(T source, T target, PropertiesSettings settings)
            : this(source, target, settings, settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops ? new RefCountCollection<ReferencePair, IPropertySynchronizer>() : null)
        {
            Ensure.NotSame(source, target, nameof(source), nameof(target));
            Ensure.SameType(source, target, nameof(source), nameof(target));
        }

        private PropertySynchronizer(T source, T target, PropertiesSettings settings, RefCountCollection<ReferencePair, IPropertySynchronizer> references)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(target, nameof(target));
            Copy.VerifyCanCopyPropertyValues(source?.GetType() ?? typeof(T), settings);
            references?.GetOrAdd(new ReferencePair(source, target), () => this);
            this.Settings = settings;
            Copy.PropertyValues(source, target, settings);
            this.propertiesSynchronizer = PropertiesSynchronizer.Create(source, target, settings, references);
            this.itemsSynchronizer = ItemsSynchronizer.Create(source, target, settings, references);
        }

        public PropertiesSettings Settings { get; }

        public void Dispose()
        {
            this.propertiesSynchronizer?.Dispose();
            this.itemsSynchronizer?.Dispose();
        }
    }
}