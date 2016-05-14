namespace Gu.State
{
    using System;
    using System.Reflection;

    internal sealed class PropertyNode : IDisposable
    {
        internal readonly PropertyInfo PropertyInfo;
        private ChangeTrackerNode node;

        public PropertyNode(Func<ChangeTrackerNode> node, PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
            this.node = node();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}