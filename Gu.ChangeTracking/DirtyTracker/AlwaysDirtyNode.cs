namespace Gu.ChangeTracking
{
    using System.Reflection;

    internal class AlwaysDirtyNode : IDirtyTrackerNode
    {
        public AlwaysDirtyNode(IDirtyTrackerNode parent, PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
            parent.Update(this);
        }

        public bool IsDirty => true;

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