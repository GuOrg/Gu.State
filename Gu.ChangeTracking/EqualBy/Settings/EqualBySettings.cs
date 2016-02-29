namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class EqualBySettings : IEqualBySettings
    {
        private readonly IgnoredTypes ignoredTypes;

        protected EqualBySettings(BindingFlags bindingFlags, ReferenceHandling referenceHandling, IEnumerable<Type> ignoredTypes)
        {
            this.BindingFlags = bindingFlags;
            this.ReferenceHandling = referenceHandling;
            this.ignoredTypes = IgnoredTypes.Create(ignoredTypes);
        }

        public BindingFlags BindingFlags { get; }

        public ReferenceHandling ReferenceHandling { get; }

        public bool IsIgnoringType(Type type)
        {
            return this.ignoredTypes.IsIgnoringType(type);
        }
    }
}