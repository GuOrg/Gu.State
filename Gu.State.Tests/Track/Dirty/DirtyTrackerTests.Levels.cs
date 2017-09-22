namespace Gu.State.Tests
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class Levels
        {
            [Test]
            public void HandlesNull()
            {
                var x = new Level { Next = new Level { Next = new Level { Name = "a" } } };
                var y = new Level { Next = new Level { Next = new Level { Name = "a" } } };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Next.Next = null;
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    var expected = "Level Next Next x: null y: Gu.State.Tests.DirtyTrackerTypes+Level";
                    var actual = tracker.Diff.ToString(string.Empty, " ");
                    Assert.AreEqual(expected, actual);

                    y.Next.Next = null;
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);

                    x.Next = new Level();
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);

                    y.Next = new Level();
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);

                    x.Next.Next = new Level();
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "Level Next Next x: Gu.State.Tests.DirtyTrackerTypes+Level y: null";
                    actual = tracker.Diff.ToString(string.Empty, " ");
                    Assert.AreEqual(expected, actual);

                    y.Next.Next = new Level();
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);

                    x.Next.Next.Name = "a";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "Level Next Next Name x: a y: null";
                    actual = tracker.Diff.ToString(string.Empty, " ");
                    Assert.AreEqual(expected, actual);

                    y.Next.Next.Name = "a";
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            public void TracksNested(ReferenceHandling referenceHandling)
            {
                var x = new Level { Next = new Level { Next = new Level { Name = "a" } } };
                var y = new Level { Next = new Level { Next = new Level { Name = "a" } } };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Name = "newName1";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual("Level Name x: newName1 y: null", tracker.Diff.ToString(string.Empty, " "));

                    y.Name = "newName1";
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);

                    x.Next.Next.Name = "b";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual("Level Next Next Name x: b y: a", tracker.Diff.ToString(string.Empty, " "));

                    y.Next.Next.Name = "b";
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);
                }
            }
        }
    }
}