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
            public void ParentChildCreateWhenParentDirtyLoop()
            {
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                var x = new Parent { Name = "p1", Child = new Child("c") };
                x.Child.Parent = x;
                var y = new Parent { Name = "p2", Child = new Child("c") };
                y.Child.Parent = y;
                using (var tracker = Track.IsDirty(x, y, ReferenceHandling.StructuralWithReferenceLoops))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);

                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.IsEmpty(changes);
                    var expected = "Parent Name x: p1 y: p2 Child Parent ...";
                    var actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    x.Name = y.Name;
                    y.Name = x.Name;
                    expectedChanges.Add("Diff", "IsDirty");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(false, tracker.IsDirty);
                }
            }

            [Test]
            public void ParentChildRenameParentLast()
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
                    var expected = "Parent Name x: Poppa y: null";
                    var actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    x.Child = new Child("Child");
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "Parent Name x: Poppa y: null Child x: Gu.State.Tests.ChangeTrackerTypes+Child y: null";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    x.Child.Parent = x;
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "Parent Name x: Poppa y: null Child x: Gu.State.Tests.ChangeTrackerTypes+Child y: null";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.Child = new Child("Child");
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "Parent Name x: Poppa y: null Child Parent x: Gu.State.Tests.ChangeTrackerTypes+Parent y: null";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.Child.Parent = y;
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "Parent Name x: Poppa y: null Child Parent ...";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.Name = x.Name;
                    expectedChanges.Add("Diff", "IsDirty");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);
                    Assert.AreSame(x.Child, x.Child.Parent.Child);
                    Assert.AreSame(x, x.Child.Parent);
                    Assert.AreSame(y.Child, y.Child.Parent.Child);
                    Assert.AreSame(y, y.Child.Parent);
                }
            }

            [Test]
            public void WithParentChildRenameParentLast()
            {
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                var x = new With<Parent>();
                var y = new With<Parent>();
                using (var tracker = Track.IsDirty(x, y, ReferenceHandling.StructuralWithReferenceLoops))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Name = "Root";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    var expected = "With<Parent> Name x: Root y: null";
                    var actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    x.Value = new Parent("Poppa1", null);
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "With<Parent> Name x: Root y: null Value x: Gu.State.Tests.ChangeTrackerTypes+Parent y: null";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.Value = new Parent("Poppa2", null);
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "With<Parent> Name x: Root y: null Value Name x: Poppa1 y: Poppa2";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    x.Value.Child = new Child("Child1");
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "With<Parent> Name x: Root y: null Value Name x: Poppa1 y: Poppa2 Child x: Gu.State.Tests.ChangeTrackerTypes+Child y: null";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.Value.Child = new Child("Child2");
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "With<Parent> Name x: Root y: null Value Name x: Poppa1 y: Poppa2 Child Name x: Child1 y: Child2";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    x.Value.Child.Parent = x.Value;
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "With<Parent> Name x: Root y: null Value Name x: Poppa1 y: Poppa2 Child Name x: Child1 y: Child2 Parent x: Gu.State.Tests.ChangeTrackerTypes+Parent y: null";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.Value.Child.Parent = y.Value;
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "With<Parent> Name x: Root y: null Value Name x: Poppa1 y: Poppa2 Child Name x: Child1 y: Child2 Parent ...";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.Value.Child.Name = x.Value.Child.Name;
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "With<Parent> Name x: Root y: null Value Name x: Poppa1 y: Poppa2 Child Parent ...";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.Name = x.Name;
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "With<Parent> Value Name x: Poppa1 y: Poppa2 Child Parent ...";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.Value.Name = x.Value.Name;
                    expectedChanges.Add("Diff", "IsDirty");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);
                }
            }

            [Test]
            public void ParentChildRenameChildLast()
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
                    var expected = "Parent Name x: Poppa y: null";
                    var actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    x.Child = new Child("ChildX");
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "Parent Name x: Poppa y: null Child x: Gu.State.Tests.ChangeTrackerTypes+Child y: null";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    x.Child.Parent = x;
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "Parent Name x: Poppa y: null Child x: Gu.State.Tests.ChangeTrackerTypes+Child y: null";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.Child = new Child("ChildY");
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "Parent Name x: Poppa y: null Child Name x: ChildX y: ChildY Parent x: Gu.State.Tests.ChangeTrackerTypes+Parent y: null";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.Child.Parent = y;
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "Parent Name x: Poppa y: null Child Name x: ChildX y: ChildY Parent ...";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.Name = x.Name;
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "Parent Child Name x: ChildX y: ChildY Parent ...";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.Child.Name = x.Child.Name;
                    expectedChanges.Add("Diff", "IsDirty");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);
                }
            }
        }
    }
}