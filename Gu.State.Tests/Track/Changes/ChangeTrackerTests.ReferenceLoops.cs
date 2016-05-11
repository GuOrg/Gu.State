// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class ChangeTrackerTests
    {
        public class ReferenceLoops
        {
            [Test]
            public void ParentChild()
            {
                var changes = new List<object>();
                var parent = new Parent();
                using (var tracker = Track.Changes(parent, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    parent.Name = "Poppa";
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    parent.Child = new Child("Child");
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    parent.Child.Parent = parent;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(3), changes);

                    parent.Name += "meh";
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(4), changes);
                }
            }
        }
    }
}