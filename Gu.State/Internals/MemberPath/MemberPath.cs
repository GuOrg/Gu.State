namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    [DebuggerDisplay("PropertyPath: {PathString}")]
    internal class MemberPath : IEnumerable<PathItem>
    {
        private static readonly PathItem[] EmptyPath = new PathItem[0];

        internal MemberPath(Type rootType)
            : this(new RootItem(rootType), EmptyPath)
        {
        }

        internal MemberPath(RootItem root, IReadOnlyList<PathItem> path)
        {
            this.Root = root;
            this.Path = path ?? EmptyPath;
        }

        internal RootItem Root { get; }

        internal Type RootType => this.Root.Type;

        internal IReadOnlyList<PathItem> Path { get; }

        internal Type LastNodeType => this.Path.OfType<ITypedNode>().LastOrDefault()?.Type ?? this.RootType;

        internal string PathString
        {
            get
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
        }

        internal MemberInfo LastMember => this.Path.OfType<IMemberItem>().LastOrDefault()?.Member;

        public IEnumerator<PathItem> GetEnumerator()
        {
            return this.Path.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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

        public MemberPath WithCollectionItem(Type collectionType)
        {
            return new MemberPath(this.Root, this.Path.Concat(new[] { new CollectionItem(collectionType) }).ToArray());
        }

        internal MemberPath WithProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                return this;
            }

            return new MemberPath(this.Root, this.Path.Concat(new[] { new PropertyItem(propertyInfo) }).ToArray());
        }

        internal MemberPath WithField(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                return this;
            }

            return new MemberPath(this.Root, this.Path.Concat(new[] { new FieldItem(fieldInfo) }).ToArray());
        }

        internal MemberPath WithIndex(int? index)
        {
            return new MemberPath(this.Root, this.Path.Concat(new[] { new IndexItem(index) }).ToArray());
        }

        internal bool Contains(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                return false;
            }

            return this.Path.OfType<IMemberItem>()
                       .Any(x => x.Member == memberInfo);
        }
    }
}
