namespace Gu.State
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    public static partial class Synchronize
    {
        private sealed class Synchronizer : IDisposable
        {
            private readonly IRefCounted<DirtyTrackerNode> dirtyTrackerNode;

            public Synchronizer(INotifyPropertyChanged source, INotifyPropertyChanged target, PropertiesSettings settings)
            {
                Ensure.NotNull(source, nameof(source));
                Ensure.NotNull(target, nameof(target));
                Ensure.NotSame(source, target, nameof(source), nameof(target));
                Ensure.SameType(source, target);
                Track.VerifyCanTrackIsDirty(source.GetType(), settings);
                Copy.VerifyCanCopyPropertyValues(source.GetType(), settings);
                this.Settings = settings;
                Copy.PropertyValues(source, target, settings);
                this.dirtyTrackerNode = DirtyTrackerNode.GetOrCreate(source, target, settings, true);
                this.dirtyTrackerNode.Value.Changed += this.OnDirtyTrackerNodeChanged;
                if (this.dirtyTrackerNode.Value.IsDirty)
                {
                    // this could happen if another thread updated source or target between creating and sunscribing to the tracker
                    // Keeping it simple and copying all again here.
                    Copy.PropertyValues(source, target, settings);
                }
            }

            public PropertiesSettings Settings { get; }

            public void Dispose()
            {
                this.dirtyTrackerNode.Value.Changed -= this.OnDirtyTrackerNodeChanged;
                this.dirtyTrackerNode.Dispose();
            }

            private void OnDirtyTrackerNodeChanged(object sender, DirtyTrackerChangedEventArgs e)
            {
                var root = e.Root;
                if (!root.Node.IsDirty)
                {
                    return;
                }

                var propertyInfo = root.MemberOrIndex as PropertyInfo;
                if (propertyInfo != null)
                {
                    if (this.Settings.IsIgnoringProperty(propertyInfo))
                    {
                        return;
                    }

                    Copy.Member(root.Node.X, root.Node.Y, this.Settings, propertyInfo);
                }
                else if (root.MemberOrIndex is int)
                {
                    throw new NotImplementedException("Get list copyer and copy index");
                }
                else
                {
                    throw Throw.ExpectedParameterOfTypes<PropertyInfo, int>("OnNodeChanged failed");
                }
            }
        }
    }
}