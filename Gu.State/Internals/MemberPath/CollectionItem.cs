namespace Gu.State
{
    using System;
    using System.Collections;

    internal class CollectionItem : PathItem, ITypedNode
    {
        internal CollectionItem(Type collectionType)
        {
            Ensure.IsAssignableFrom<IEnumerable>(collectionType, nameof(collectionType));
            this.CollectionType = collectionType;
        }

        Type ITypedNode.Type => this.CollectionType.GetItemType();

        internal Type CollectionType { get; }
    }
}