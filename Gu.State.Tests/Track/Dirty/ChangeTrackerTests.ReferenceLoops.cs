namespace Gu.State.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class ReferenceLoops
        {
            [Test]
            public void ParentChild()
            {
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                var x = new Parent();
                var y = new Parent();
                using (var tracker = Track.IsDirty(x, y, ReferenceHandling.StructuralWithReferenceLoops))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Name = "Poppa";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual("Parent Name x: Poppa y: null", tracker.Diff.ToString("", " "));

                    x.Child = new Child("Child");
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual("Parent Name x: Poppa y: null Child x: Gu.State.Tests.ChangeTrackerTypes+Child y: null", tracker.Diff.ToString("", " "));

                    x.Child.Parent = x;
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual("Parent Name x: Poppa y: null Child x: Gu.State.Tests.ChangeTrackerTypes+Child y: null", tracker.Diff.ToString("", " "));

                    y.Child = new Child("Child");
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual("Parent Name x: Poppa y: null", tracker.Diff.ToString("", " "));

                    y.Child.Parent = y;
                    expectedChanges.Add("Diff", "IsDirty");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);
                }
            }
        }
    }
}