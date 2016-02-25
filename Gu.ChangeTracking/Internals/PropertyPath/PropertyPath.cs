namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    [DebuggerDisplay("PropertyPath: {PathString}")]
    internal class PropertyPath
    {
        private static readonly PathItem[] EmptyPath = new PathItem[0];

        internal PropertyPath(Type rootType)
            : this(new RootItem(rootType), EmptyPath)
        {
        }

        internal PropertyPath(RootItem root, IReadOnlyList<PathItem> path)
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
                    var propertyItem = pathItem as PropertyItem;
                    if (propertyItem != null)
                    {
                        stringBuilder.Append('.');
                        stringBuilder.Append(propertyItem.Property.Name);
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

        internal PropertyPath WithProperty(PropertyInfo propertyInfo)
        {
            return new PropertyPath(this.Root, this.Path.Concat(new[] { new PropertyItem(propertyInfo) }).ToArray());
        }

        internal PropertyPath WithIndex(int? index)
        {
            return new PropertyPath(this.Root, this.Path.Concat(new[] { new IndexItem(index) }).ToArray());
        }
    }
}
