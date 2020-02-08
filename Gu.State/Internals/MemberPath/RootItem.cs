namespace Gu.State
{
    using System;

    internal class RootItem : PathItem
    {
        internal RootItem(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; }
    }
}
