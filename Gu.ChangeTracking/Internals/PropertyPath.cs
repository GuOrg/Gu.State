namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal class PropertyPath
    {
        private static readonly PathItem[] EmptyPath = new PathItem[0];

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
                stringBuilder.Append(this.Root.Type.Name);
                foreach (var pathItem in this.Path)
                {
                    var propertyItem = pathItem as PropertyItem;
                    if (propertyItem != null)
                    {
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

        internal static PropertyPath Create(ChangeTracker tracker)
        {
            var pathItems = new List<PathItem>();
            while (true)
            {
                if (tracker.GetType() == typeof(ChangeTracker))
                {
                    var rootItem = new RootItem(tracker.Source);
                    return new PropertyPath(rootItem, pathItems);
                }

                var propertyChangeTracker = tracker as PropertyChangeTracker;
                if (propertyChangeTracker != null)
                {
                    var propertyItem = new PropertyItem(propertyChangeTracker.PropertyInfo);
                    pathItems.Insert(0, propertyItem);
                    continue;
                }

                var itemChangeTracker = tracker as ItemChangeTracker;
                if (itemChangeTracker != null)
                {
                    var indexItem = new IndexItem(itemChangeTracker.Index);
                    pathItems.Insert(0, indexItem);
                    continue;
                }

                throw new ArgumentOutOfRangeException();
            }
        }

        internal PropertyPath WithProperty(PropertyInfo propertyInfo)
        {
            return new PropertyPath(this.Root, this.Path.Concat(new[] { new PropertyItem(propertyInfo) }).ToArray());
        }

        internal abstract class PathItem
        {
        }

        internal class RootItem : PathItem
        {
            private readonly object root;

            public RootItem(object root)
            {
                this.root = root;
            }

            public Type Type => this.root?.GetType();
        }

        internal class PropertyItem : PathItem
        {
            public PropertyItem(PropertyInfo property)
            {
                this.Property = property;
            }

            public PropertyInfo Property { get; }
        }

        internal class IndexItem : PathItem
        {
            public IndexItem(int index)
            {
                this.Index = index;
            }

            public int Index { get; }
        }
    }
}
