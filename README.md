# Gu.State
Library for managing state.

## Track
Tracks changes in a graph.
For subproperties the following must hold:
- Collections must implement INotifyCollectionChanged
- Types that are not collections and not immutable must implement INotifyPropertyChanged.
- Indexers are only supported for framework collection types.

##### Changes.

```
using (var tracker = Track.Changes(foo))
{
    Assert.AreEqual(0, tracker.Changes);
    foo.SomeProperty.NestedCollection[0].Value++;
    Assert.AreEqual(1, tracker.Changes);
}
// no longer tracking after disposing.
```

##### IsDirty.

```
using (var tracker = Track.IsDirty(x, y))
{
    Assert.AreEqual(false, tracker.IsDirty);
    foo.SomeProperty.NestedCollection[0].Value++;
    Assert.AreEqual(true, tracker.IsDirty);
}
// no longer tracking after disposing.
```

##### Verify.
```
Track.VerifyCanTrackIsDirty<T>(ReferenceHandling.Structural);
Track.VerifyCanTrackChanges<T>(ReferenceHandling.Structural);
```
Use the verify methods in unit tests to assert that your types support tracking and that the correct settings are used.

##### PropertiesSettings.
For more finegrained control there is an overload accepting a `PropertiesSettings`
```
var settings = new PropertiesSettingsBuilder().IgnoreProperty<WithIllegal>(x => x.Illegal)
                                              .CreateSettings(ReferenceHandling.Structural);
using (var tracker = Track.Changes(withIllegalObject, settings))
{
    ...
}
```

## Copy
Copies values from source to target.
Immutable types are copied by value/reference.
- Indexers are only supported for framework collection types.
- Collections must implement `IList` or `IDictionary`
#### FieldValues
```
Copy.FieldValues(source, target);
Copy.FieldValues(source, target, ReferenceHandling.Structural); 
Copy.FieldValues(source, target, ReferenceHandling.StructuralWithReferenceLoops); 
Copy.FieldValues(source, target, ReferenceHandling.References);
```
#### PropertyValues
```
Copy.PropertyValues(source, target);
Copy.PropertyValues(source, target, ReferenceHandling.Structural); 
Copy.PropertyValues(source, target, ReferenceHandling.StructuralWithReferenceLoops); 
Copy.PropertyValues(source, target, ReferenceHandling.References);
```
##### PropertiesSettings.
For more finegrained control there is an overload accepting a `PropertiesSettings`

## EqualBy
Compares two instances.
Types implementing `IEquatable` are compared using `object.Equals(x, y)`
- Indexers are only supported for framework collection types.
- Handles enumerables.


#### FieldValues
```
EqualBy.FieldValues(source, target);
EqualBy.FieldValues(source, target, ReferenceHandling.Structural); 
EqualBy.FieldValues(source, target, ReferenceHandling.StructuralWithReferenceLoops); 
EqualBy.FieldValues(source, target, ReferenceHandling.References);
```
- Ignores event fields

#### PropertyValues
```
EqualBy.PropertyValues(source, target);
EqualBy.PropertyValues(source, target, ReferenceHandling.Structural); 
EqualBy.PropertyValues(source, target, ReferenceHandling.StructuralWithReferenceLoops); 
EqualBy.PropertyValues(source, target, ReferenceHandling.References);
```

##### PropertiesSettings.
For more finegrained control there is an overload accepting a `PropertiesSettings`

## PropertySynchronizer
Keeps the property values of target in sync with source.
For subproperties the following must hold:
- Collections must implement INotifyCollectionChanged
- Types that are not collections and not immutable must implement INotifyPropertyChanged.
- Indexers are only supported for framework collection types.
```
using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.Structural))
{
}
```
