# Gu.State

[![Join the chat at https://gitter.im/JohanLarsson/Gu.State](https://badges.gitter.im/JohanLarsson/Gu.State.svg)](https://gitter.im/JohanLarsson/Gu.State?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md)
[![Build status](https://ci.appveyor.com/api/projects/status/thxx3efi2p90byja/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-state/branch/master)

Library for managing state.

## Table of Contents
  - [1. EqualBy](#1-equalby)
    - [1.1. FieldValues](#11-fieldvalues)
    - [1.2. VerifyCanEqualByFieldValues](#12-verifycanequalbyfieldvalues)
    - [1.3. PropertyValues](#13-propertyvalues)
    - [1.4. VerifyCanEqualByPropertyValues](#14-verifycanequalbypropertyvalues)
  - [2. Copy](#2-copy)
    - [2.1. FieldValues](#21-fieldvalues)
    - [2.2. VerifyCanCopyFieldValues](#22-verifycancopyfieldvalues)
    - [2.3. PropertyValues](#23-propertyvalues)
    - [2.4. VerifyCanCopyPropertyValues](#24-verifycancopypropertyvalues)
  - [3. DiffBy](#3-diffby)
    - [3.1. FieldValues](#31-fieldvalues)
    - [3.4. VerifyCanDiffByFieldValues](#34-verifycandiffbyfieldvalues)
    - [3.2. PropertyValues](#32-propertyvalues)
    - [3.4. VerifyCanDiffByPropertyValues](#34-verifycandiffbypropertyvalues)
  - [4. Track](#4-track)
    - [4.1. Changes.](#41-changes)
    - [4.2 Track.VerifyCanTrackChanges](#42-trackverifycantrackchanges)
    - [4.3. IsDirty.](#43-isdirty)
    - [4.4. Track.VerifyCanTrackIsDirty](#44-trackverifycantrackisdirty)
  - [5. Synchronize](#5-synchronize)
  - [6. Settings.](#6-settings)
    - [6.1. PropertiesSettings.](#61-propertiessettings)
    - [6.2. FieldsSettings.](#62-fieldssettings)

## 1. EqualBy
Compares two instances.

- Types implementing `IEquatable` are compared using `object.Equals(x, y)`
- Indexers are only supported for framework collection types like `List<T>`.
- Handles enumerables.

### 1.1. FieldValues
Compares x and y by field values.
```
EqualBy.FieldValues(x, y); // default is ReferenceHandling.Structural
EqualBy.FieldValues(x, y, ReferenceHandling.Throw);  
EqualBy.FieldValues(x, y, ReferenceHandling.Structural); 
EqualBy.FieldValues(x, y, ReferenceHandling.References);
EqualBy.FieldValues(x, y, settings); // settings should be cached between calls for performance.
```

- Ignores event fields

### 1.2. VerifyCanEqualByFieldValues

Asserts that instances of type `<T>` can be compared using the `EqualBy.FieldValues` method.
This can be useful in unit tests.
Throws an exception with a message describing the problem(s) found and suggestions for fixes.
```
EqualBy.VerifyCanEqualByFieldValues<T>(); // default is ReferenceHandling.Structural
EqualBy.VerifyCanEqualByFieldValues<T>(ReferenceHandling.Structural); 
EqualBy.VerifyCanEqualByFieldValues<T>(ReferenceHandling.References);
EqualBy.VerifyCanEqualByFieldValues<T>(ReferenceHandling.Throw); 
EqualBy.VerifyCanEqualByFieldValues<T>(settings); // settings should be cached between calls for performance.
```

### 1.3. PropertyValues
```
EqualBy.PropertyValues(x, y);
EqualBy.PropertyValues(x, y, ReferenceHandling.Structural); 
EqualBy.PropertyValues(x, y, ReferenceHandling.References);
EqualBy.PropertyValues(x, y, ReferenceHandling.Throw); 
EqualBy.PropertyValues(x, y, settings); // settings should be cached between calls for performance.
```

### 1.4. VerifyCanEqualByPropertyValues

Asserts that instances of type `<T>` can be compared using the `EqualBy.PropertyValues` method.
This can be useful in unit tests.
Throws an exception with a message describing the problem(s) found and suggestions for fixes.
```
EqualBy.VerifyCanEqualByPropertyValues<T>();
EqualBy.VerifyCanEqualByPropertyValues<T>(ReferenceHandling.Structural); 
EqualBy.VerifyCanEqualByPropertyValues<T>(ReferenceHandling.References);
EqualBy.VerifyCanEqualByPropertyValues<T>(ReferenceHandling.Throw); 
EqualBy.VerifyCanEqualByPropertyValues<T>(settings); // settings should be cached between calls for performance.
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
Copy.FieldValues(source, target, ReferenceHandling.References);
Copy.FieldValues(source, target, ReferenceHandling.Throw); 
```

### 2.2. VerifyCanCopyFieldValues

Asserts that instances of type `<T>` can be copied using the `Copy.FieldValues` method.
This can be useful in unit tests.
Throws an exception with a message describing the problem(s) found and suggestions for fixes.
```
Copy.VerifyCanCopyFieldValues<T>(); // default is ReferenceHandling.Structural
Copy.VerifyCanCopyFieldValues<T>(ReferenceHandling.Structural); 
Copy.VerifyCanCopyFieldValues<T>(ReferenceHandling.References);
Copy.VerifyCanCopyFieldValues<T>(ReferenceHandling.Throw); 
Copy.VerifyCanCopyFieldValues<T>(settings); // settings should be cached between calls for performance.
```

### 2.3. PropertyValues
```
Copy.PropertyValues(source, target);
Copy.PropertyValues(source, target, ReferenceHandling.Structural); 
Copy.PropertyValues(source, target, ReferenceHandling.References);
Copy.PropertyValues(source, target, ReferenceHandling.Throw); 
```

### 2.4. VerifyCanCopyPropertyValues

Asserts that instances of type `<T>` can be copied using the `Copy.PropertyValues` method.
This can be useful in unit tests.
Throws an exception with a message describing the problem(s) found and suggestions for fixes.
```
Copy.VerifyCanCopyPropertyValues<T>(); // default is ReferenceHandling.Structural
Copy.VerifyCanCopyPropertyValues<T>(ReferenceHandling.Structural); 
Copy.VerifyCanCopyPropertyValues<T>(ReferenceHandling.References);
Copy.VerifyCanCopyPropertyValues<T>(ReferenceHandling.Throw); 
Copy.VerifyCanCopyPropertyValues<T>(settings); // settings should be cached between calls for performance.
```

## 3. DiffBy
Compares two instances and returns a tree with the differences or an empty diff is they are equal.
Types implementing `IEquatable` are compared using `object.Equals(x, y)`
- Indexers are only supported for framework collection types.
- Handles enumerables.

### 3.1. FieldValues
```
DiffBy.FieldValues(x, y);
DiffBy.FieldValues(x, y, ReferenceHandling.Structural); 
DiffBy.FieldValues(x, y, ReferenceHandling.References);
DiffBy.FieldValues(x, y, ReferenceHandling.Throw); 
DiffBy.FieldValues(x, y, settings); // settings should be cached between calls for performance.
```
- Ignores event fields

### 3.4. VerifyCanDiffByFieldValues

Asserts that instances of type `<T>` can be diffed using the `DiffBy.FieldValues` method.
This can be useful in unit tests.
Throws an exception with a message describing the problem(s) found and suggestions for fixes.
```
DiffBy.VerifyCanDiffByFieldValues<T>(); // default is ReferenceHandling.Structural
DiffBy.VerifyCanDiffByFieldValues<T>(ReferenceHandling.Structural); 
DiffBy.VerifyCanDiffByFieldValues<T>(ReferenceHandling.References);
DiffBy.VerifyCanDiffByFieldValues<T>(ReferenceHandling.Throw); 
DiffBy.VerifyCanDiffByFieldValues<T>(settings); // settings should be cached between calls for performance.
```

### 3.2. PropertyValues
```
DiffBy.PropertyValues(x, y);
DiffBy.PropertyValues(x, y, ReferenceHandling.Structural); 
DiffBy.PropertyValues(x, y, ReferenceHandling.References);
DiffBy.PropertyValues(x, y, ReferenceHandling.Throw); 
DiffBy.PropertyValues(x, y, settings); // settings should be cached between calls for performance.
```

### 3.4. VerifyCanDiffByPropertyValues

Asserts that instances of type `<T>` can be diffed using the `DiffBy.PropertyValues` method.
This can be useful in unit tests.
Throws an exception with a message describing the problem(s) found and suggestions for fixes.
```
DiffBy.VerifyCanDiffByPropertyValues<T>(); // default is ReferenceHandling.Structural
DiffBy.VerifyCanDiffByPropertyValues<T>(ReferenceHandling.Structural); 
DiffBy.VerifyCanDiffByPropertyValues<T>(ReferenceHandling.References);
DiffBy.VerifyCanDiffByPropertyValues<T>(ReferenceHandling.Throw); 
DiffBy.VerifyCanDiffByPropertyValues<T>(settings); // settings should be cached between calls for performance.
```

## 4. Track
    
### 4.1. Changes.
Tracks changes in a graph.
For subproperties the following must hold:
- Collections must implement INotifyCollectionChanged
- Types that are not collections and not immutable must implement INotifyPropertyChanged.
- Indexers are only supported for framework collection types.

```
using (var tracker = Track.Changes(foo))
{
    Assert.AreEqual(0, tracker.Changes);
    foo.SomeProperty.NestedCollection[0].Value++;
    Assert.AreEqual(1, tracker.Changes);
}
// no longer tracking after disposing.
```
### 4.2 Track.VerifyCanTrackChanges

Asserts that changes for instances of type `<T>` can be tracked using the `Track.Changes` method.
This can be useful in unit tests.
Throws an exception with a message describing the problem(s) found and suggestions for fixes.
```
Track.VerifyCanTrackChanges<T>(); // default is ReferenceHandling.Structural
Track.VerifyCanTrackChanges<T>(ReferenceHandling.Structural); 
Track.VerifyCanTrackChanges<T>(ReferenceHandling.References);
Track.VerifyCanTrackChanges<T>(ReferenceHandling.Throw); 
Track.VerifyCanTrackChanges<T>(settings); // settings should be cached between calls for performance.
```

### 4.3. IsDirty.
Tracks the difference bwtween two instances.
For subproperties the following must hold:
- Collections must implement INotifyCollectionChanged
- Types that are not collections and not immutable must implement INotifyPropertyChanged.
- Indexers are only supported for framework collection types.

```
using (var tracker = Track.IsDirty(x, y))
{
    Assert.AreEqual(false, tracker.IsDirty);
    foo.SomeProperty.NestedCollection[0].Value++;
    Assert.AreEqual(true, tracker.IsDirty);
}
// no longer tracking after disposing.
```

### 4.4. Track.VerifyCanTrackIsDirty

Asserts that dirty can be tracked for instances of type `<T>` using the `Track.Changes` method.
This can be useful in unit tests.
Throws an exception with a message describing the problem(s) found and suggestions for fixes.
```
Track.VerifyCanTrackIsDirty<T>(); // default is ReferenceHandling.Structural
Track.VerifyCanTrackIsDirty<T>(ReferenceHandling.Structural); 
Track.VerifyCanTrackIsDirty<T>(ReferenceHandling.References);
Track.VerifyCanTrackIsDirty<T>(ReferenceHandling.Throw); 
Track.VerifyCanTrackIsDirty<T>(settings); // settings should be cached between calls for performance.
```

## 5. Synchronize
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

## 6. Settings.
For more finegrained control the above methods have an overload accepting a settings object.
Cache the setting between calls for performance.

### 6.1. PropertiesSettings.
For convenience there is a builder exposed that can be used to create the setting.
```
var settings = PropertiesSettings.Build()
                                 .IgnoreProperty<Foo>(x => x.IgnoredProperty)
                                 .IgnoreType<Bar>()
                                 .AddImmutableType<Baz>()
                                 .AddComparer(new CustomComparer())
                                 .AddCopyer(new CustomCopyer())
                                 .CreateSettings(ReferenceHandling.Structural);
using (var tracker = Track.Changes(foo, settings))
{
    ...
}
```
### 6.2. FieldsSettings.
Same as PropertiesSettings but for methods operating on fields.


