namespace Gu.ChangeTracking
{
    using System.Reflection;

    public abstract class CopySettings
    {
        protected CopySettings(BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            this.BindingFlags = bindingFlags;
            this.ReferenceHandling = referenceHandling;
        }
        public BindingFlags BindingFlags { get; }

        public ReferenceHandling ReferenceHandling { get; }
    }
}