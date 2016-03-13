namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    [DebuggerDisplay("PropertyPath: {PathString()}")]
    internal class MemberPath : IEnumerable<PathItem>
    {
        private static readonly PathItem[] EmptyPath = new PathItem[0];

        internal MemberPath(Type rootType)
            : this(new RootItem(rootType), EmptyPath)
        {
        }

        internal MemberPath(RootItem root, IEnumerable<PathItem> path)
        {
            this.Root = root;
            this.Path = path ?? EmptyPath;
        }

        internal RootItem Root { get; }

        internal Type RootType => this.Root.Type;

        internal IEnumerable<PathItem> Path { get; }

        internal Type LastNodeType => this.Path.OfType<ITypedNode>().LastOrDefault()?.Type ?? this.RootType;

        internal MemberInfo LastMember => this.Path.OfType<IMemberItem>().LastOrDefault()?.Member;

        public IEnumerator<PathItem> GetEnumerator()
        {
            return this.Path.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal string PathString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(this.Root.Type.PrettyName());
            foreach (var pathItem in this.Path)
            {
                var propertyItem = pathItem as IMemberItem;
                if (propertyItem != null)
                {
                    stringBuilder.Append('.')
                                 .Append(propertyItem.Member.Name);
                    continue;
                }

                var collectionItem = pathItem as CollectionItem;
                if (collectionItem != null)
                {
                    stringBuilder.Append("[")
                                 .Append(collectionItem.CollectionType.GetItemType().PrettyName())
                                 .Append("]");
                    continue;
                }

                var indexItem = pathItem as IndexItem;
                if (indexItem != null)
                {
                    stringBuilder.Append($"[{indexItem.Index}]");
                    continue;
                }

                throw Throw.ShouldNeverGetHereException();
            }

            return stringBuilder.ToString();
        }

        internal MemberPath WithMember(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                return this;
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return this.WithField(fieldInfo);
            }

            var propertyInfo = memberInfo as PropertyInfo;

            if (propertyInfo != null)
            {
                return this.WithProperty(propertyInfo);
            }

            throw Throw.ExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(memberInfo));
        }

        internal MemberPath WithCollectionItem(Type collectionType)
        {
            return new MemberPath(this.Root, this.Path.Append(new CollectionItem(collectionType)));
        }

        internal MemberPath WithProperty(PropertyInfo propertyInfo)
        {
            Debug.Assert(propertyInfo != null, nameof(propertyInfo));
            //Debug.Assert(this.LastNodeType.GetProperties().Contains(propertyInfo) != false, "Must contain property");
            return new MemberPath(this.Root, this.Path.Append(new PropertyItem(propertyInfo)));
        }

        internal MemberPath WithField(FieldInfo fieldInfo)
        {
            Debug.Assert(fieldInfo != null, nameof(fieldInfo));
            //Debug.Assert(this.LastNodeType.GetFields().Contains(fieldInfo) != false, "Must contain property");
            return new MemberPath(this.Root, this.Path.Append(new FieldItem(fieldInfo)));
        }

        internal MemberPath WithIndex(int? index)
        {
            return new MemberPath(this.Root, this.Path.Append(new IndexItem(index)));
        }

        public bool HasLoop()
        {
            var lastOrDefault = this.Path.OfType<IMemberItem>()
                                    .LastOrDefault();
            if (lastOrDefault == null)
            {
                return false;
            }

            foreach (var item in this.Path.OfType<IMemberItem>().SkipLast())
            {
                if (item.Member == lastOrDefault.Member)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
