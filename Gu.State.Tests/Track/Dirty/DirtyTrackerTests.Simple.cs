namespace Gu.State.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class Simple
        {
            [Test]
            public void CreateAndDispose()
            {
                var x = new SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var y = new SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();

                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Value1++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value1 x: 2 y: 1", tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }

                x.Value1++;
                CollectionAssert.AreEqual(expectedChanges, changes);

#if (!DEBUG) // debug build keeps instances alive longer for nicer debugging experience
                var wrx = new System.WeakReference(x);
                var wry = new System.WeakReference(y);
                x = null;
                y = null;
                System.GC.Collect();
                Assert.AreEqual(false, wrx.IsAlive);
                Assert.AreEqual(false, wry.IsAlive);
#endif
            }

            [Test]
            public void DoesNotNotifyWhenNoChangeWhenNotDirty()
            {
                var x = new SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var y = new SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var changes = new List<string>();

                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.OnPropertyChanged(nameof(SimpleDirtyTrackClass.Value1));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff?.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    x.OnPropertyChanged(nameof(SimpleDirtyTrackClass.Value2));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff?.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    y.OnPropertyChanged(nameof(SimpleDirtyTrackClass.Value1));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff?.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    y.OnPropertyChanged(nameof(SimpleDirtyTrackClass.Value2));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff?.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);
                }
            }

            [Test]
            public void DoesNotNotifyWhenNoChangeWhenDirty()
            {
                var x = new SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var y = new SimpleDirtyTrackClass { Value1 = 1, Value2 = 3 };
                var changes = new List<string>();
                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value2 x: 2 y: 3", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    x.OnPropertyChanged(nameof(SimpleDirtyTrackClass.Value1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value2 x: 2 y: 3", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    x.OnPropertyChanged(nameof(SimpleDirtyTrackClass.Value2));
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value2 x: 2 y: 3", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);


                    y.OnPropertyChanged(nameof(SimpleDirtyTrackClass.Value1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value2 x: 2 y: 3", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    y.OnPropertyChanged(nameof(SimpleDirtyTrackClass.Value2));
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value2 x: 2 y: 3", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);
                }
            }

            [Test]
            public void TracksX()
            {
                var x = new SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var y = new SimpleDirtyTrackClass { Value1 = 3, Value2 = 4 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();

                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value1 x: 1 y: 3 Value2 x: 2 y: 4", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    x.Value1 = 5;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value1 x: 5 y: 3 Value2 x: 2 y: 4", tracker.Diff.ToString("", " "));
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Value1 = 3;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value2 x: 2 y: 4", tracker.Diff.ToString("", " "));
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Value2 = 4;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Value2 = 3;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value2 x: 3 y: 4", tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void TracksY()
            {
                var x = new SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var y = new SimpleDirtyTrackClass { Value1 = 3, Value2 = 4 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value1 x: 1 y: 3 Value2 x: 2 y: 4", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    y.Value1 = 5;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value1 x: 1 y: 5 Value2 x: 2 y: 4", tracker.Diff.ToString("", " "));
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Value1 = 1;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value2 x: 2 y: 4", tracker.Diff.ToString("", " "));
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Value2 = 2;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff?.ToString("", " "));
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Value2 = 3;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value2 x: 2 y: 3", tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }
        }
    }
}