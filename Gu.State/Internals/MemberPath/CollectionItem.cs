namespace Gu.State
{
    using System;
    using System.Collections;

    internal class CollectionItem : PathItem, ITypedNode
    {
        public CollectionItem(Type collectionType)
        {
            Ensure.IsAssignableFrom<IEnumerable>(collectionType, nameof(collectionType));
            this.CollectionType = collectionType;
        }

        public Type CollectionType { get; }

        Type ITypedNode.Type => this.CollectionType.GetItemType();
    }
}