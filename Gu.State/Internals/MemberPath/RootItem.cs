namespace Gu.State
{
    using System;

    internal class RootItem : PathItem
    {
        internal RootItem(Type type)
        {
            this.Type = type;
        }

        internal Type Type { get; }
    }
}
