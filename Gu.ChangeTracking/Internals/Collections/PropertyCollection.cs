namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    internal class PropertyCollection
    {
        private readonly IReadOnlyList<PropertyAndDisposable> items;

        internal PropertyCollection(IReadOnlyList<PropertyAndDisposable> items)
        {
            this.items = items;
        }

        public bool Contains(PropertyInfo propertyInfo)
        {
            foreach (var item in this.items)
            {
                if (item.PropertyInfo == propertyInfo)
                {
                    return true;
                }
            }

            return false;
        }

        internal IDisposable this[PropertyInfo index]
        {
            get { return this.items.Single(x => x.PropertyInfo == index).Synchronizer; }
            set { this.items.Single(x => x.PropertyInfo == index).Synchronizer = value; }
        }

        public void Dispose()
        {
            if (this.items == null)
            {
                return;
            }

            foreach (var item in this.items)
            {
                item?.Dispose();
            }
        }

        [DebuggerDisplay("PropertyAndDisposable, Property: {PropertyInfo.Name} ")]
        internal class PropertyAndDisposable : IDisposable
        {
            private readonly object gate = new object();
            private IDisposable synchronizer;

            public PropertyAndDisposable(PropertyInfo propertyInfo, IDisposable synchronizer)
            {
                this.PropertyInfo = propertyInfo;
                this.synchronizer = synchronizer;
            }

            public PropertyInfo PropertyInfo { get; }

            public IDisposable Synchronizer
            {
                get
                {
                    lock (this.gate)
                    {
                        return this.synchronizer;
                    }
                }
                set
                {
                    lock (this.gate)
                    {
                        this.synchronizer?.Dispose();
                        this.synchronizer = value;
                    }
                }
            }

            public void Dispose()
            {
                this.Synchronizer?.Dispose();
            }
        }
    }
}