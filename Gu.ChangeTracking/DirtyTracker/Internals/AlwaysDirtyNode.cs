namespace Gu.ChangeTracking
{
    using System.Reflection;

    internal class AlwaysDirtyNode : IDirtyTrackerNode
    {
        public AlwaysDirtyNode(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
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