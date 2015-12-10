using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Settings for how <see cref="ChangeTracker"/> tracks changes.
    /// </summary>
    [Serializable]
    public class ChangeTrackerSettings
    {
        private readonly ConcurrentDictionary<string, SpecialType> specialTypes = new ConcurrentDictionary<string, SpecialType>();
        private readonly ConcurrentDictionary<PropertyInfo, SpecialProperty> specialProperties = new ConcurrentDictionary<PropertyInfo, SpecialProperty>();
        
        /// <summary>
        /// The default change tracker settings containing common ignores like for <see cref="System.Type"/>.
        /// </summary>
        public static ChangeTrackerSettings Default => CreateDefault();

        /// <summary>
        /// The types that the <see cref="ChangeTracker"/> ignores.
        /// </summary>
        public IEnumerable<SpecialType> SpecialTypes => this.specialTypes.Values;

        /// <summary>
        /// The properties that the <see cref="ChangeTracker"/> ignores.
        /// </summary>
        public IEnumerable<SpecialProperty> SpecialProperties => this.specialProperties.Values;

        /// <summary>
        /// Adds a special type <see cref="TrackAs.Immutable"/> for <typeparamref name="T"/>.
        /// This means that the <see cref="ChangeTracker"/> will not track changes to items of this type.
        /// </summary>
        /// <typeparam name="T">The immutable type</typeparam>
        public void AddImmutableType<T>()
        {
            AddSpecialType<T>(TrackAs.Immutable);
        }

        /// <summary>
        /// Adds a special type <see cref="TrackAs.Immutable"/> for <paramref name="type"/>.
        /// This means that the <see cref="ChangeTracker"/> will not track changes to items of this type.
        /// </summary>
        public void AddImmutableType(Type type)
        {
            AddSpecialType(type, TrackAs.Immutable);
        }

        /// <summary>
        /// Adds a special type <see cref="TrackAs.Explicit"/> for <typeparamref name="T"/>.
        /// This means that the <see cref="ChangeTracker"/> will not track changes to items of this type.
        /// </summary>
        /// <typeparam name="T">The Explicit type</typeparam>
        public void AddExplicitType<T>()
        {
            AddSpecialType<T>(TrackAs.Explicit);
        }

        /// <summary>
        /// Adds a special type <see cref="TrackAs.Explicit"/> for <paramref name="type"/>.
        /// This means that the <see cref="ChangeTracker"/> will not track changes to items of this type.
        /// </summary>
        public void AddExplicitType(Type type)
        {
            AddSpecialType(type, TrackAs.Explicit);
        }

        /// <summary>
        /// Adds a special type <see cref="TrackAs.Explicit"/> for <paramref name="property"/>.
        /// This means that the <see cref="ChangeTracker"/> will not track changes to items of this type.
        /// </summary>
        public void AddExplicitProperty(PropertyInfo property)
        {
            var specialProperty = new SpecialProperty(property, TrackAs.Explicit);
            if (!this.specialProperties.TryAdd(property, specialProperty))
            {
                var message = $"Failed adding {property.DeclaringType?.FullName}{property.Name}. {nameof(SpecialProperties)} already contains key {specialProperty.Name}";
                throw new InvalidOperationException(message);
            }
        }

        public void AddSpecialType<T>(TrackAs trackas)
        {
            AddSpecialType(typeof(T), trackas);
        }

        public void AddSpecialType(Type type, TrackAs trackas)
        {
            var specialType = new SpecialType(type, trackas);
            if (!this.specialTypes.TryAdd(specialType.Name, specialType))
            {
                var message = $"Failed adding {type.FullName}. {nameof(SpecialTypes)} already contains key {specialType.Name}";
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Gets if <paramref name="type"/> should be tracked.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if <paramref name="type"/> is ignored when tracking changes.</returns>
        public bool IsIgnored(Type type)
        {
            return this.specialTypes.ContainsKey(type.FullName);
        }

        /// <summary>
        /// Gets if <paramref name="property"/> should be tracked.
        /// </summary>
        /// <param name="property">The type.</param>
        /// <returns>True if <paramref name="property"/> is ignored when tracking changes.</returns>
        public bool IsIgnored(PropertyInfo property)
        {
            return this.specialProperties.ContainsKey(property);
        }

        private static ChangeTrackerSettings CreateDefault()
        {
            var settings = new ChangeTrackerSettings();
            settings.AddSpecialType<FileInfo>(TrackAs.Explicit);
            settings.AddSpecialType<DirectoryInfo>(TrackAs.Explicit);
            settings.AddSpecialType<Type>(TrackAs.Immutable);
            settings.AddSpecialType<CultureInfo>(TrackAs.Immutable);
            settings.AddSpecialType<DateTime>(TrackAs.Immutable);
            settings.AddSpecialType<DateTime?>(TrackAs.Immutable);
            settings.AddSpecialType<DateTimeOffset>(TrackAs.Immutable);
            settings.AddSpecialType<DateTimeOffset?>(TrackAs.Immutable);
            settings.AddSpecialType<TimeSpan>(TrackAs.Immutable);
            settings.AddSpecialType<TimeSpan?>(TrackAs.Immutable);
            settings.AddSpecialType<string>(TrackAs.Immutable);
            settings.AddSpecialType<double?>(TrackAs.Immutable);
            settings.AddSpecialType<float?>(TrackAs.Immutable);
            settings.AddSpecialType<decimal?>(TrackAs.Immutable);
            settings.AddSpecialType<int?>(TrackAs.Immutable);
            settings.AddSpecialType<uint?>(TrackAs.Immutable);
            settings.AddSpecialType<long?>(TrackAs.Immutable);
            settings.AddSpecialType<ulong?>(TrackAs.Immutable);
            settings.AddSpecialType<short?>(TrackAs.Immutable);
            settings.AddSpecialType<ushort?>(TrackAs.Immutable);
            settings.AddSpecialType<sbyte?>(TrackAs.Immutable);
            settings.AddSpecialType<byte?>(TrackAs.Immutable);
            return settings;
        }
    }
}
