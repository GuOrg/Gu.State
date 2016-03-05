namespace Gu.ChangeTracking
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

        internal IReadOnlyList<PathItem> Path { get; }

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
                        stringBuilder.Append('.');
                        stringBuilder.Append(propertyItem.Member.Name);
                        continue;
                    }

                    var indexItem = pathItem as IndexItem;
                    if (indexItem != null)
                    {
                        stringBuilder.Append($"[{indexItem.Index}]");
                        continue;
                    }

                    throw new ArgumentOutOfRangeException();
                }

                return stringBuilder.ToString();
            }
        }

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

        internal MemberPath WithProperty(PropertyInfo propertyInfo)
        {
            return new MemberPath(this.Root, this.Path.Concat(new[] { new PropertyItem(propertyInfo) }).ToArray());
        }

        internal MemberPath WithField(FieldInfo fieldInfo)
        {
            return new MemberPath(this.Root, this.Path.Concat(new[] { new FieldItem(fieldInfo) }).ToArray());
        }

        internal MemberPath WithIndex(int? index)
        {
            return new MemberPath(this.Root, this.Path.Concat(new[] { new IndexItem(index) }).ToArray());
        }

        internal bool Contains(MemberInfo memberInfo)
        {
            return this.Path.OfType<IMemberItem>()
                       .Any(x => x.Member == memberInfo);
        }
    }
}
