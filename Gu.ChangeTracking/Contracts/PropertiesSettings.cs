namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    public class PropertiesSettings : MemberSettings<PropertyInfo>, IIgnoringProperties
    {
        private static readonly ConcurrentDictionary<BindingFlagsAndReferenceHandling, PropertiesSettings> Cache = new ConcurrentDictionary<BindingFlagsAndReferenceHandling, PropertiesSettings>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesSettings"/> class.
        /// </summary>
        /// <param name="ignoredProperties">The properties provided here will be ignored when the intsance of <see cref="PropertiesSettings"/> is used</param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        public PropertiesSettings(IEnumerable<PropertyInfo> ignoredProperties, BindingFlags bindingFlags, ReferenceHandling referenceHandling)
            : base(ignoredProperties, bindingFlags, referenceHandling)
        {
        }

        public IEnumerable<PropertyInfo> IgnoredProperties => this.IgnoredMembers;

        /// <summary>
        /// Creates an instance of <see cref="PropertiesSettings"/>.
        /// </summary>
        /// <typeparam name="T">The type to get ignore properties for settings for</typeparam>
        /// <param name="x">The instance to copy property values from</param>
        /// <param name="y">The instance to copy property values to</param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        /// <param name="ignoreProperties">Names of properties on <typeparamref name="T"/> to exclude from copying</param>
        /// <returns>An instance of <see cref="PropertiesSettings"/></returns>
        public static PropertiesSettings Create<T>(T x, T y, BindingFlags bindingFlags, ReferenceHandling referenceHandling = ReferenceHandling.Throw, params string[] ignoreProperties)
        {
            var type = x?.GetType() ?? y?.GetType() ?? typeof(T);
            return Create(type, bindingFlags, referenceHandling, ignoreProperties);
        }

        /// <summary>
        /// Creates an instance of <see cref="PropertiesSettings"/>.
        /// </summary>
        /// <typeparam name="T">The type to get ignore properties for settings for</typeparam>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        /// <param name="ignoreProperties">Names of properties on <typeparamref name="T"/> to exclude from copying</param>
        /// <returns>An instance of <see cref="PropertiesSettings"/></returns>
        public static PropertiesSettings Create<T>(BindingFlags bindingFlags, ReferenceHandling referenceHandling = ReferenceHandling.Throw, params string[] ignoreProperties)
        {
            return Create(typeof(T), bindingFlags, referenceHandling, ignoreProperties);
        }

        /// <summary>
        /// Creates an instance of <see cref="PropertiesSettings"/>.
        /// </summary>
        /// <param name="type">The type to get ignore properties for settings for</param>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        /// <param name="ignoreProperties">Names of properties on <paramref name="type"/> to exclude from copying</param>
        /// <returns>An instance of <see cref="PropertiesSettings"/></returns>
        public static PropertiesSettings Create(Type type, BindingFlags bindingFlags, ReferenceHandling referenceHandling = ReferenceHandling.Throw, params string[] ignoreProperties)
        {
            var ignored = type.GetIgnoreProperties(bindingFlags, ignoreProperties);
            if (ignored == null || ignored.Count == 0)
            {
                return GetOrCreate(bindingFlags, referenceHandling);
            }

            return new PropertiesSettings(ignored, bindingFlags, referenceHandling);
        }

        /// <summary>
        /// Creates an instance of <see cref="PropertiesSettings"/>.
        /// </summary>
        /// <param name="bindingFlags">The binding flags to use when getting properties</param>
        /// <param name="referenceHandling">
        /// If Structural is used property values for sub properties are copied for the entire graph.
        /// Activator.CreateInstance is sued to new up references so a default constructor is required, can be private
        /// </param>
        /// <returns>An instance of <see cref="PropertiesSettings"/></returns>
        public static PropertiesSettings GetOrCreate(BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags, ReferenceHandling referenceHandling = ReferenceHandling.Throw)
        {
            var key = new BindingFlagsAndReferenceHandling(bindingFlags, referenceHandling);
            return Cache.GetOrAdd(key, x => new PropertiesSettings(null, bindingFlags, referenceHandling));
        }

        public bool IsIgnoringProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                return true;
            }

            if (this.IsIgnoringDeclaringType(propertyInfo.DeclaringType))
            {
                return true;
            }

            return this.IsIgnoringMember(propertyInfo);
        }

        public SpecialCopyProperty GetSpecialCopyProperty(PropertyInfo propertyInfo)
        {
            return null;
        }
    }
}
