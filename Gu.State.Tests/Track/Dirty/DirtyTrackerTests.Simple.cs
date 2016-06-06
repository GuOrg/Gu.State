namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class Simple
        {
            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.References)]
            [TestCase(ReferenceHandling.Structural)]
            public void CreateAndDispose(ReferenceHandling referenceHandling)
            {
                var x = new WithSimpleProperties { Value = 1, Time = DateTime.MinValue };
                var y = new WithSimpleProperties { Value = 1, Time = DateTime.MinValue };
                var propertyChanges = new List<string>();
                var expectedChanges = new List<string>();

                using (var tracker = Track.IsDirty(x, y, referenceHandling))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(propertyChanges);

                    x.Value++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Value x: 2 y: 1", tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, propertyChanges);
                }

                x.Value++;
                CollectionAssert.AreEqual(expectedChanges, propertyChanges);

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

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.References)]
            [TestCase(ReferenceHandling.Structural)]
            public void CreateAndDisposeExplicitSetting(ReferenceHandling referenceHandling)
            {
                var x = new WithSimpleProperties { Value = 1, Time = DateTime.MinValue };
                var y = new WithSimpleProperties { Value = 1, Time = DateTime.MinValue };
                var propertyChanges = new List<string>();
                var expectedChanges = new List<string>();

                using (var tracker = Track.IsDirty(x, y, PropertiesSettings.GetOrCreate(referenceHandling)))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(propertyChanges);

                    x.Value++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Value x: 2 y: 1", tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, propertyChanges);
                }

                x.Value++;
                CollectionAssert.AreEqual(expectedChanges, propertyChanges);

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
            public void DoesNotNotifyWhenMissingProperty()
            {
                var x = new WithSimpleProperties { Value = 1, Time = DateTime.MinValue };
                var y = new WithSimpleProperties { Value = 1, Time = DateTime.MinValue };
                var propertyChanges = new List<string>();

                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(propertyChanges);

                    x.OnPropertyChanged("Missing");
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff?.ToString("", " "));
                    CollectionAssert.IsEmpty(propertyChanges);

                    y.OnPropertyChanged("Missing");
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff?.ToString("", " "));
                    CollectionAssert.IsEmpty(propertyChanges);
                }
            }

            [Test]
            public void DoesNotNotifyWhenNoChangeWhenNotDirty()
            {
                var x = new WithSimpleProperties { Value = 1, Time = DateTime.MinValue };
                var y = new WithSimpleProperties { Value = 1, Time = DateTime.MinValue };
                var propertyChanges = new List<string>();

                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(propertyChanges);

                    x.OnPropertyChanged(nameof(WithSimpleProperties.Value));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff?.ToString("", " "));
                    CollectionAssert.IsEmpty(propertyChanges);

                    x.OnPropertyChanged(nameof(WithSimpleProperties.Time));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff?.ToString("", " "));
                    CollectionAssert.IsEmpty(propertyChanges);

                    y.OnPropertyChanged(nameof(WithSimpleProperties.Value));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff?.ToString("", " "));
                    CollectionAssert.IsEmpty(propertyChanges);

                    y.OnPropertyChanged(nameof(WithSimpleProperties.Time));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff?.ToString("", " "));
                    CollectionAssert.IsEmpty(propertyChanges);
                }
            }

            [Test]
            public void DoesNotNotifyWhenNoChangeWhenDirty()
            {
                var x = new WithSimpleProperties { Value = 1, Time = DateTime.MinValue };
                var y = new WithSimpleProperties { Value = 1, Time = DateTime.MinValue.AddSeconds(1) };
                var propertyChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Time x: 0001-01-01 00:00:00 y: 0001-01-01 00:00:01", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(propertyChanges);

                    x.OnPropertyChanged(nameof(WithSimpleProperties.Value));
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Time x: 0001-01-01 00:00:00 y: 0001-01-01 00:00:01", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(propertyChanges);

                    x.OnPropertyChanged(nameof(WithSimpleProperties.Time));
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Time x: 0001-01-01 00:00:00 y: 0001-01-01 00:00:01", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(propertyChanges);

                    y.OnPropertyChanged(nameof(WithSimpleProperties.Value));
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Time x: 0001-01-01 00:00:00 y: 0001-01-01 00:00:01", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(propertyChanges);

                    y.OnPropertyChanged(nameof(WithSimpleProperties.Time));
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Time x: 0001-01-01 00:00:00 y: 0001-01-01 00:00:01", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(propertyChanges);
                }
            }

            [Test]
            public void TracksX()
            {
                var x = new WithSimpleProperties { Value = 1, Time = DateTime.MinValue };
                var y = new WithSimpleProperties { Value = 3, Time = DateTime.MinValue.AddSeconds(1) };
                var propertyChanges = new List<string>();
                var expectedChanges = new List<string>();

                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Time x: 0001-01-01 00:00:00 y: 0001-01-01 00:00:01 Value x: 1 y: 3", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(propertyChanges);

                    x.Value = 5;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Time x: 0001-01-01 00:00:00 y: 0001-01-01 00:00:01 Value x: 5 y: 3", tracker.Diff.ToString("", " "));
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, propertyChanges);

                    x.Value = 3;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Time x: 0001-01-01 00:00:00 y: 0001-01-01 00:00:01", tracker.Diff.ToString("", " "));
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, propertyChanges);

                    x.Time = y.Time;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, propertyChanges);

                    x.Time = DateTime.MinValue.AddSeconds(2);
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Time x: 0001-01-01 00:00:02 y: 0001-01-01 00:00:01", tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, propertyChanges);
                }
            }

            [Test]
            public void TracksY()
            {
                var x = new WithSimpleProperties { Value = 1, Time = DateTime.MinValue };
                var y = new WithSimpleProperties { Value = 3, Time = DateTime.MinValue.AddSeconds(1) };
                var propertyChanges = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Time x: 0001-01-01 00:00:00 y: 0001-01-01 00:00:01 Value x: 1 y: 3", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(propertyChanges);

                    y.Value = 5;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Time x: 0001-01-01 00:00:00 y: 0001-01-01 00:00:01 Value x: 1 y: 5", tracker.Diff.ToString("", " "));
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, propertyChanges);

                    y.Value = 1;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Time x: 0001-01-01 00:00:00 y: 0001-01-01 00:00:01", tracker.Diff.ToString("", " "));
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, propertyChanges);

                    y.Time = DateTime.MinValue;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff?.ToString("", " "));
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, propertyChanges);

                    y.Time = DateTime.MinValue.AddSeconds(2);
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Time x: 0001-01-01 00:00:00 y: 0001-01-01 00:00:02", tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, propertyChanges);
                }
            }

            [TestCase(null)]
            [TestCase("")]
            public void HandlesPropertyChangedEmptyAndNull(string prop)
            {
                var x = new WithSimpleProperties(1, DateTime.MinValue);
                var y = new WithSimpleProperties(1, DateTime.MinValue);
                var propertyChanges = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(propertyChanges);

                    x.SetFields(1, DateTime.MinValue.AddSeconds(1));
                    x.OnPropertyChanged(prop);
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithSimpleProperties Time x: 0001-01-01 00:00:01 y: 0001-01-01 00:00:00", tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, propertyChanges);
                }
            }
        }
    }
}