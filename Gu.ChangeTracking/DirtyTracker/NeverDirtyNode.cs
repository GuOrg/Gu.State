namespace Gu.ChangeTracking
{
    using System.Reflection;

    internal class NeverDirtyNode : IDirtyTrackerNode
    {
        public NeverDirtyNode(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
        }

        public bool IsDirty => false;

        public PropertyInfo PropertyInfo { get; }

        public void Dispose()
        {
            // nop
        }

        public void Update(IDirtyTrackerNode child)
        {
            // nop
        }
    }
}