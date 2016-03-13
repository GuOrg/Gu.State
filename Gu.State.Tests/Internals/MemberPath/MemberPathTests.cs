namespace Gu.State.Tests.Internals
{
    using System;
    using System.Reflection;
    using NUnit.Framework;

    public class MemberPathTests
    {
        private static readonly PropertyInfo ChildProperty = typeof(MemberPathTypes.Parent).GetProperty(nameof(MemberPathTypes.Parent.Child));
        private static readonly PropertyInfo ParentProperty = typeof(MemberPathTypes.Child).GetProperty(nameof(MemberPathTypes.Child.Parent));

        [Test]
        public void PathString()
        {
            var path = new MemberPath(typeof(MemberPathTypes.With<MemberPathTypes.Parent>));
            Assert.AreEqual("With<Parent>", path.PathString());

            path = path.WithProperty(ChildProperty);
            Assert.AreEqual("With<Parent>.Child", path.PathString());

            path = path.WithProperty(ParentProperty);
            Assert.AreEqual("With<Parent>.Child.Parent", path.PathString());
        }

        [Test]
        public void LastMember()
        {
            var rootType = typeof(MemberPathTypes.With<MemberPathTypes.Parent>);
            var path1 = new MemberPath(rootType);
            Assert.AreEqual(rootType, path1.Root.Type);
            Assert.AreEqual(null, path1.LastMember);

            var path2 = path1.WithProperty(ChildProperty);
            Assert.AreNotSame(path1, path2);
            Assert.AreEqual(rootType, path2.Root.Type);
            Assert.AreEqual(ChildProperty, path2.LastMember);

            var path3 = path2.WithProperty(ParentProperty);
            Assert.AreEqual(rootType, path3.Root.Type);
            Assert.AreEqual(ParentProperty, path3.LastMember);
        }

        [Test]
        public void WithLoop()
        {
            var rootType = typeof(MemberPathTypes.With<MemberPathTypes.Parent>);
            var valueProperty = rootType.GetProperty(nameof(MemberPathTypes.With<MemberPathTypes.Parent>.Value));
            var childProperty = typeof(MemberPathTypes.Parent).GetProperty(nameof(MemberPathTypes.Parent.Child));
            var parentProperty = typeof(MemberPathTypes.Child).GetProperty(nameof(MemberPathTypes.Child.Parent));
            var path = new MemberPath(rootType)
                            .WithProperty(valueProperty)
                            .WithProperty(childProperty)
                            .WithProperty(parentProperty);
            Assert.AreEqual(false, path.HasLoop());

            path = path.WithProperty(childProperty);
            Assert.AreEqual(true, path.HasLoop());
        }
    }
}
