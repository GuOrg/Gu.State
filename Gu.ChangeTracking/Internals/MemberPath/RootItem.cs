namespace Gu.ChangeTracking
{
    using System;

    internal class RootItem : PathItem
    {
        public RootItem(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; }
    }
}