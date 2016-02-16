namespace Gu.ChangeTracking
{
    using System.Reflection;

    public abstract class EqualBySettings
    {
        protected EqualBySettings(BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            this.BindingFlags = bindingFlags;
            this.ReferenceHandling = referenceHandling;
        }
        public BindingFlags BindingFlags { get; }

        public ReferenceHandling ReferenceHandling { get; }
    }
}