namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class PropertiesSettingsBuilder
    {
        private readonly Dictionary<Type, TrackAs> specialTypes = new Dictionary<Type, TrackAs>();
        private readonly Dictionary<Type, TrackAs> specialProperties = new Dictionary<Type, TrackAs>();

        public PropertiesSettingsBuilder AddImmutableType<T>()
        {
            this.AddSpecialType<T>(TrackAs.Immutable);
            return this;
        }

        public PropertiesSettingsBuilder AddSpecialType<T>(TrackAs trackas)
        {
            this.AddSpecialType(typeof(T), trackas);
            return this;
        }

        public PropertiesSettingsBuilder AddSpecialType(Type type, TrackAs trackas)
        {
            this.specialTypes.Add(type, trackas);
            return this;
        }

        public PropertiesSettingsBuilder AddIgnoredProperty(PropertyInfo property)
        {
            
        }
    }
}
