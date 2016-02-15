# Gu.ChangeTracking
Small library for tracking changes to object graphs.

## ChangeTracker
Tracks changes in a graph, handles collections and nested properties.
##### Sample 1, simplest use case.

```
using (var tracker = ChangeTracker.Track(foo))
{
    Assert.AreEqual(0, tracker.Changes);
    foo.SomeProperty.NestedCollection[0].Value++;
    Assert.AreEqual(1, tracker.Changes);
}
```

##### Sample 2, ignore immutable type

```
var settings = ChangeTrackerSettings.Default;
settings.AddImmutableType<Color>(); // we tell the tracker that Color is immutable like this.
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

##### Sample 3, ignore property
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

## DirtyTracker
Tracks changes to two instances of the same type and reports if they are different.
Useful for edit views where a copy is edited and compared to the instance last saved to disk.
The `DirtyTracker` is `IDisposable` and disposing stops listening.
Notifies when there are changes to `IsDirty` and `Diff` via `INotifyPropertyChanged`
Only simple properties { IEquatable<struct>, string } are supported for now.
The constructor checks and throws. There is also a Vefify method that can be used to check if a type can be tracked.
Sample:

```c#
var dirtyTracker = DirtyTracker.Track(x, y);
// dirtyTracker.IsDirty true if any property differs between x and y. 
```

## Copy

#### FieldValues
```
Copy.FieldValues(source, target);
Copy.FieldValues(source, target, "ignoreThisField");
Copy.FieldValues(source, target, ReferenceHandling.Structural); // copies the field values for nested reference types. Used Activator.CreateInstance so a default ctor is required, can be private.
Copy.FieldValues(source, target, ReferenceHandling.Reference); // copies references for nested reference types.
```
#### PropertyValues
```
Copy.PropertyValues(source, target);
Copy.PropertyValues(source, target, "ignoreThisField");
Copy.PropertyValues(source, target, ReferenceHandling.Structural); // copies the field values for nested reference types. Used Activator.CreateInstance so a default ctor is required, can be private.
Copy.PropertyValues(source, target, ReferenceHandling.Reference); // copies references for nested reference types.
```

## PropertySynchronizer
Keeps the property values of target in sync with source.
