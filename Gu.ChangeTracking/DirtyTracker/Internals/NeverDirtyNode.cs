namespace Gu.ChangeTracking
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("NeverDirtyNode Property: {PropertyInfo.Name}")]
    internal class NeverDirtyNode : IDirtyTrackerNode
    {
        private static readonly Dictionary<PropertyInfo, NeverDirtyNode> Cache = new Dictionary<PropertyInfo, NeverDirtyNode>();

        private NeverDirtyNode(PropertyInfo propertyInfo)
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

        internal static NeverDirtyNode For(PropertyInfo propertyInfo)
        {
            NeverDirtyNode node;
            if (Cache.TryGetValue(propertyInfo, out node))
            {
                return node;
            }

            node = new NeverDirtyNode(propertyInfo);
            Cache[propertyInfo] = node;
            return node;
        }
    }
}