namespace Gu.ChangeTracking
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("Item: {Value.ToString()} ParentType:{ParentType.Name} ParentProperty: {ParentProperty.Name}")]
    internal abstract class PropertyTracker : ValueTracker, IPropertyTracker
    {
        protected PropertyTracker(Type parentType, PropertyInfo property, object value)
            : base(value)
        {
            Ensure.NotNull(property, nameof(property));
            this.ParentType = parentType;
            this.ParentProperty = property;
        }

        /// <inheritdoc/>
        public Type ParentType { get; }

        /// <inheritdoc/>
        public PropertyInfo ParentProperty { get; }
    }
}