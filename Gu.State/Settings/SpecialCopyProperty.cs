namespace Gu.State
{
    using System;
    using System.Reflection;

    public class SpecialCopyProperty : ISpecialCopyProperty
    {
        private readonly Action<object, object> copyValue;

        public SpecialCopyProperty(PropertyInfo property, Action<object, object> copyValue)
        {
            this.copyValue = copyValue;
            this.Property = property;
        }

        public PropertyInfo Property { get; }

        public void CopyValue(object source, object target)
        {
            this.copyValue(source, target);
        }
    }
}