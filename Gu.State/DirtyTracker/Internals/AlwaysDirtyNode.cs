namespace Gu.State
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("AlwaysDirtyNode Property: {PropertyInfo.Name}")]
    internal class AlwaysDirtyNode : IDirtyTrackerNode
    {
        private static readonly Dictionary<PropertyInfo, AlwaysDirtyNode> Cache = new Dictionary<PropertyInfo, AlwaysDirtyNode>();

        private AlwaysDirtyNode(PropertyInfo propertyInfo)
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

        internal static AlwaysDirtyNode For(PropertyInfo propertyInfo)
        {
            AlwaysDirtyNode node;
            if (Cache.TryGetValue(propertyInfo, out node))
            {
                return node;
            }

            node = new AlwaysDirtyNode(propertyInfo);
            Cache[propertyInfo] = node;
            return node;
        }
    }
}