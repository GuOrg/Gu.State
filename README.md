# Gu.State
Library for managing state.

## Table of Contents
- [1. EqualBy](#1-equalby)
- [1.1. FieldValues](#11-fieldvalues)
- [1.2. VerifyCanEqualByFieldValues](#12-verifycanequalbyfieldvalues)
- [1.3. PropertyValues](#13-propertyvalues)
- [1.4. VerifyCanEqualByPropertyValues](#14-verifycanequalbypropertyvalues)
- [2. Copy](#2-copy)
- [2.1. FieldValues](#21-fieldvalues)
- [2.2. PropertyValues](#22-propertyvalues)
- [3. DiffBy](#3-diffby)
- [3.1. FieldValues](#31-fieldvalues)
- [3.2. PropertyValues](#32-propertyvalues)

## 1. EqualBy
Compares two instances.

- Types implementing `IEquatable` are compared using `object.Equals(x, y)`
- Indexers are only supported for framework collection types like `List<T>`.
- Handles enumerables.

### 1.1. FieldValues
Compares x and y by field values.
```
EqualBy.FieldValues(x, y);
EqualBy.FieldValues(x, y, ReferenceHandling.Structural); 
EqualBy.FieldValues(x, y, ReferenceHandling.StructuralWithReferenceLoops); 
EqualBy.FieldValues(x, y, ReferenceHandling.References);
EqualBy.FieldValues(x, y, settings);
```

- Ignores event fields

### 1.2. VerifyCanEqualByFieldValues

Asserts that instances of type `<T>` can be compared using the `EqualBy.FieldValues` method.
This can be useful in unit tests.
Throws an exception with a message describing the problem(s) found and suggestions for fixes.
```
EqualBy.VerifyCanEqualByFieldValues<T>();
EqualBy.VerifyCanEqualByFieldValues<T>(ReferenceHandling.Structural); 
EqualBy.VerifyCanEqualByFieldValues<T>(ReferenceHandling.StructuralWithReferenceLoops); 
EqualBy.VerifyCanEqualByFieldValues<T>(ReferenceHandling.References);
EqualBy.VerifyCanEqualByFieldValues<T>(settings);
```

### 1.3. PropertyValues
```
EqualBy.PropertyValues(x, y);
EqualBy.PropertyValues(x, y, ReferenceHandling.Structural); 
EqualBy.PropertyValues(x, y, ReferenceHandling.StructuralWithReferenceLoops); 
EqualBy.PropertyValues(x, y, ReferenceHandling.References);
EqualBy.PropertyValues(x, y, settings);
```

### 1.4. VerifyCanEqualByPropertyValues

Asserts that instances of type `<T>` can be compared using the `EqualBy.PropertyValues` method.
This can be useful in unit tests.
Throws an exception with a message describing the problem(s) found and suggestions for fixes.
```
EqualBy.VerifyCanEqualByPropertyValues<T>();
EqualBy.VerifyCanEqualByPropertyValues<T>(ReferenceHandling.Structural); 
EqualBy.VerifyCanEqualByPropertyValues<T>(ReferenceHandling.StructuralWithReferenceLoops); 
EqualBy.VerifyCanEqualByPropertyValues<T>(ReferenceHandling.References);
EqualBy.VerifyCanEqualByPropertyValues<T>(settings);
```

## 2. Copy
Copies values from source to target.
Immutable types are copied by value/reference.
- Indexers are only supported for framework collection types.
- Collections must implement `IList` or `IDictionary`

### 2.1. FieldValues
```
Copy.FieldValues(source, target);
Copy.FieldValues(source, target, ReferenceHandling.Structural); 
Copy.FieldValues(source, target, ReferenceHandling.StructuralWithReferenceLoops); 
Copy.FieldValues(source, target, ReferenceHandling.References);
```

### 2.2. PropertyValues
```
Copy.PropertyValues(source, target);
Copy.PropertyValues(source, target, ReferenceHandling.Structural); 
Copy.PropertyValues(source, target, ReferenceHandling.StructuralWithReferenceLoops); 
Copy.PropertyValues(source, target, ReferenceHandling.References);
```

## 3. DiffBy
Compares two instances and returns a tree with the diff or null is they are equal.
Types implementing `IEquatable` are compared using `object.Equals(x, y)`
- Indexers are only supported for framework collection types.
- Handles enumerables.

### 3.1. FieldValues
```
DiffBy.FieldValues(x, y);
DiffBy.FieldValues(x, y, ReferenceHandling.Structural); 
DiffBy.FieldValues(x, y, ReferenceHandling.StructuralWithReferenceLoops); 
DiffBy.FieldValues(x, y, ReferenceHandling.References);
```
- Ignores event fields

### 3.2. PropertyValues
```
DiffBy.PropertyValues(x, y);
DiffBy.PropertyValues(x, y, ReferenceHandling.Structural); 
DiffBy.PropertyValues(x, y, ReferenceHandling.StructuralWithReferenceLoops); 
DiffBy.PropertyValues(x, y, ReferenceHandling.References);
```

## 4. Track
Tracks changes in a graph.
For subproperties the following must hold:
- Collections must implement INotifyCollectionChanged
- Types that are not collections and not immutable must implement INotifyPropertyChanged.
- Indexers are only supported for framework collection types.

### 4.1. Changes.

```
using (var tracker = Track.Changes(foo))
{
    Assert.AreEqual(0, tracker.Changes);
    foo.SomeProperty.NestedCollection[0].Value++;
    Assert.AreEqual(1, tracker.Changes);
}
// no longer tracking after disposing.
```

### 4.2. IsDirty.

```
using (var tracker = Track.IsDirty(x, y))
{
    Assert.AreEqual(false, tracker.IsDirty);
    foo.SomeProperty.NestedCollection[0].Value++;
    Assert.AreEqual(true, tracker.IsDirty);
}
// no longer tracking after disposing.
```

### Verify.
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

## Synchronize
Keeps the property values of target in sync with source.
For subproperties the following must hold:
- Collections must implement INotifyCollectionChanged
- Types that are not collections and not immutable must implement INotifyPropertyChanged.
- Indexers are only supported for framework collection types.
```
using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.Structural))
{
    ...
}
```

## FieldsSettings.
For more finegrained control there is an overload accepting a `FieldsSettings`


## PropertiesSettings.
For more finegrained control there is an overload accepting a `PropertiesSettings`
