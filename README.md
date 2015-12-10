# Gu.ChangeTracking

Small library for tracking changes to object graphs.

### Sample 1, simplest use case.

```
using (var tracker = ChangeTracker.Track(foo))
{
    Assert.AreEqual(0, tracker.Changes);
    foo.SomeProperty.NestedCollection[0].Value++;
    Assert.AreEqual(1, tracker.Changes);
}
```

### Sample 2, ignore immutable type

```
var settings = ChangeTrackerSettings.Default;
settings.AddImmutableType<Color>(); // we tell the tracker that COlor is immutable like this.
var foo = new WithColor();
using (var tracker = ChangeTracker.Track(foo, settings))
{
    Assert.AreEqual(0, tracker.Changes);
    foo.Name = "Johan";
    Assert.AreEqual(1, tracker.Changes);

    foo.Color = Color.Blue;
    Assert.AreEqual(1, tracker.Changes); // no change 
}
```
If we don't pass a setting we get the exception:

```
Create tracker failed for Gu.ChangeTracking.Tests.ChangeTrackerTests.PropertyChanged.WithColor.Color.
Solve the problem by any of:
* Add a specialcase to tracker setting example:
    settings.AddSpecialType<System.Drawing.Color>(...)
    or:    settings.AddSpecialProperty(typeof(WithColor).GetProperty(nameof(WithColor.Color))    Note that this requires you to track changes.
* Implement INotifyPropertyChanged for Gu.ChangeTracking.Tests.ChangeTrackerTests.PropertyChanged.WithColor
* Implement INotifyCollectionChanged for Gu.ChangeTracking.Tests.ChangeTrackerTests.PropertyChanged.WithColor
* Add attribute [IgnoreChangesAttribute] to type Gu.ChangeTracking.Tests.ChangeTrackerTests.PropertyChanged.WithColor
* Add attribute [IgnoreChangesAttribute] to property Gu.ChangeTracking.Tests.ChangeTrackerTests.PropertyChanged.WithColor.Color
```

### Sample 3, ignore property
```
var settings = new ChangeTrackerSettings();
var property = foo.GetType().GetProperty(nameof(Foo.IgnoredProperty));
settings.AddExplicitProperty(property);
using (var tracker = ChangeTracker.Track(foo, settings))
{
    Assert.AreEqual(0, tracker.Changes);
    foo.Value++;
    Assert.AreEqual(1, tracker.Changes);

    foo.IgnoredProperty++;
    Assert.AreEqual(1, tracker.Changes);
}
```
