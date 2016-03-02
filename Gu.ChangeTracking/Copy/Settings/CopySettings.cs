namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class CopySettings
    {
        private readonly IgnoredTypes ignoredTypes;

        protected CopySettings(BindingFlags bindingFlags, ReferenceHandling referenceHandling, IEnumerable<Type> ignoredTypes)
        {
            this.BindingFlags = bindingFlags;
            this.ReferenceHandling = referenceHandling;
            this.ignoredTypes = IgnoredTypes.Create(ignoredTypes);
        }

        public BindingFlags BindingFlags { get; }

        public ReferenceHandling ReferenceHandling { get; }

        public bool IsIgnoringDeclaringType(Type declaringType)
        {
            return this.ignoredTypes.IsIgnoringType(declaringType);
        }
    }
}