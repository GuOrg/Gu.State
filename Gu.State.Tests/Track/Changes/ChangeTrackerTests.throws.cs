namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class ChangeTrackerTests
    {
        public class Throws
        {
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.Throw)]
            public void WithNotifyingStruct(ReferenceHandling referenceHandling)
            {
                var expected = "Track.Changes(x, y) failed.\r\n" +
                               "The type NotifyingStruct is a mutable struct that implements INotifyPropertyChanged.\r\n" +
                               "  As it is a value type subscribing to changes does not make sense.\r\n" +
                               "The property With<NotifyingStruct>.Value of type NotifyingStruct is not supported.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Make NotifyingStruct immutable or use an immutable type.\r\n" +
                               "  - For immutable types the following must hold:\r\n" +
                               "    - Must be a sealed class or a struct.\r\n" +
                               "    - All fields and properties must be readonly.\r\n" +
                               "    - All field and property types must be immutable.\r\n" +
                               "    - All indexers must be readonly.\r\n" +
                               "    - Event fields are ignored.\r\n" +
                               "* Use PropertiesSettings and specify how change tracking is performed:\r\n" +
                               "  - ReferenceHandling.Structural means that a the entire graph is tracked.\r\n" +
                               "  - ReferenceHandling.References means that only the root level changes are tracked.\r\n" +
                               "  - Exclude a combination of the following:\r\n" +
                               "    - The property With<NotifyingStruct>.Value.\r\n" +
                               "    - The type NotifyingStruct.\r\n";

                var exception = Assert.Throws<NotSupportedException>(() => Track.Changes(new With<NotifyingStruct>(), referenceHandling));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NotSupportedException>(() => Track.Changes(new With<NotifyingStruct>()));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void WithComplexTypeThrows()
            {
                var expected = // "Track.Changes(x, y) failed for item: ObservableCollection<ComplexType>[0].Illegal.\r\n" +
                    "Track.Changes(x, y) failed.\r\n" +
                    "The property With<ComplexType>.Value of type ComplexType is not supported.\r\n" +
                    "Solve the problem by any of:\r\n" +
                    "* Make ComplexType immutable or use an immutable type.\r\n" +
                    "  - For immutable types the following must hold:\r\n" +
                    "    - Must be a sealed class or a struct.\r\n" +
                    "    - All fields and properties must be readonly.\r\n" +
                    "    - All field and property types must be immutable.\r\n" +
                    "    - All indexers must be readonly.\r\n" + 
                    "    - Event fields are ignored.\r\n" + 
                    "* Use PropertiesSettings and specify how change tracking is performed:\r\n" +
                    "  - ReferenceHandling.Structural means that a the entire graph is tracked.\r\n" + 
                    "  - ReferenceHandling.References means that only the root level changes are tracked.\r\n" +
                    "  - Exclude a combination of the following:\r\n" +
                    "    - The property With<ComplexType>.Value.\r\n" +
                    "    - The type ComplexType.\r\n";

                var exception = Assert.Throws<NotSupportedException>(() => Track.Changes(new With<ComplexType>(), ReferenceHandling.Throw));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void WithComplexTypeHappyPath()
            {
                Assert.DoesNotThrow(() => Track.Changes(new With<ComplexType>()));
                // ReSharper disable once RedundantArgumentDefaultValue
                Assert.DoesNotThrow(() => Track.Changes(new With<ComplexType>(), ReferenceHandling.Structural));
                Assert.DoesNotThrow(() => Track.Changes(new With<ComplexType>(), ReferenceHandling.References));
            }

            [Test]
            public void AddIllegalThrows()
            {
                var expected = "Track.Changes(x, y) failed.\r\n" +
                               "The type IllegalType does not notify changes.\r\n" +
                               "The property IllegalSubType.Illegal of type IllegalType is not supported.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Implement INotifyPropertyChanged for IllegalType or use a type that does.\r\n" +
                               "* Make IllegalSubType immutable or use an immutable type.\r\n" +
                               "* Make IllegalType immutable or use an immutable type.\r\n" +
                               "  - For immutable types the following must hold:\r\n" +
                               "    - Must be a sealed class or a struct.\r\n" +
                               "    - All fields and properties must be readonly.\r\n" +
                               "    - All field and property types must be immutable.\r\n" +
                               "    - All indexers must be readonly.\r\n" +
                               "    - Event fields are ignored.\r\n" +
                               "* Use PropertiesSettings and specify how change tracking is performed:\r\n" +
                               "  - ReferenceHandling.Structural means that a the entire graph is tracked.\r\n" +
                               "  - ReferenceHandling.References means that only the root level changes are tracked.\r\n" +
                               "  - Exclude a combination of the following:\r\n" +
                               "    - The property IllegalSubType.Illegal.\r\n" +
                               "    - The type IllegalSubType.\r\n" +
                               "    - The type IllegalType.\r\n";

                var root = new ObservableCollection<ComplexType>();
                using (Track.Changes(root))
                {
                    var exception = Assert.Throws<NotSupportedException>(() => root.Add(new IllegalSubType()));
                    Assert.AreEqual(expected, exception.Message);
                }
            }

            [Test]
            public void SetIllegalThrows()
            {
                var expected = // "Track.Changes(x, y) failed for item: ObservableCollection<ComplexType>[0].Illegal.\r\n" +
                               "Track.Changes(x, y) failed.\r\n" +
                               "The type IllegalType does not notify changes.\r\n" +
                               "The property IllegalSubType.Illegal of type IllegalType is not supported.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Implement INotifyPropertyChanged for IllegalType or use a type that does.\r\n" +
                               "* Make IllegalSubType immutable or use an immutable type.\r\n" +
                               "* Make IllegalType immutable or use an immutable type.\r\n" +
                               "  - For immutable types the following must hold:\r\n" +
                               "    - Must be a sealed class or a struct.\r\n" +
                               "    - All fields and properties must be readonly.\r\n" +
                               "    - All field and property types must be immutable.\r\n" +
                               "    - All indexers must be readonly.\r\n" +
                               "    - Event fields are ignored.\r\n" +
                               "* Use PropertiesSettings and specify how change tracking is performed:\r\n" +
                               "  - ReferenceHandling.Structural means that a the entire graph is tracked.\r\n" +
                               "  - ReferenceHandling.References means that only the root level changes are tracked.\r\n" +
                               "  - Exclude a combination of the following:\r\n" +
                               "    - The property IllegalSubType.Illegal.\r\n" +
                               "    - The type IllegalSubType.\r\n" +
                               "    - The type IllegalType.\r\n";

                var root = new With<ComplexType>();
                using (Track.Changes(root))
                {
                    var exception = Assert.Throws<NotSupportedException>(() => root.Value = new IllegalSubType());
                    Assert.AreEqual(expected, exception.Message);
                }
            }

            [Test]
            public void WithIllegal()
            {
                var expected = // "Track.Changes(x, y) failed for item: ObservableCollection<ComplexType>[0].Illegal.\r\n" +
                               "Track.Changes(x, y) failed.\r\n" +
                               "The type IllegalType does not notify changes.\r\n" +
                               "The property WithIllegal.Illegal of type IllegalType is not supported.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Implement INotifyPropertyChanged for IllegalType or use a type that does.\r\n" +
                               "* Make IllegalType immutable or use an immutable type.\r\n" +
                               "  - For immutable types the following must hold:\r\n" +
                               "    - Must be a sealed class or a struct.\r\n" +
                               "    - All fields and properties must be readonly.\r\n" +
                               "    - All field and property types must be immutable.\r\n" +
                               "    - All indexers must be readonly.\r\n" +
                               "    - Event fields are ignored.\r\n" +
                               "* Use PropertiesSettings and specify how change tracking is performed:\r\n" +
                               "  - ReferenceHandling.Structural means that a the entire graph is tracked.\r\n" +
                               "  - ReferenceHandling.References means that only the root level changes are tracked.\r\n" +
                               "  - Exclude a combination of the following:\r\n" +
                               "    - The property WithIllegal.Illegal.\r\n" +
                               "    - The type IllegalType.\r\n";

                var item = new WithIllegal();
                var settings = PropertiesSettings.GetOrCreate();
                var exception = Assert.Throws<NotSupportedException>(() => Track.Changes(item, settings));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NotSupportedException>(() => Track.Changes(item));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void IllegalEnumerable()
            {
                var expected = "Track.Changes(x, y) failed.\r\n" +
                               "The collection type IllegalEnumerable does not notify changes.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Implement INotifyCollectionChanged for IllegalEnumerable or use a type that does.\r\n" +
                               "* Use PropertiesSettings and specify how change tracking is performed:\r\n" +
                               "  - ReferenceHandling.Structural means that a the entire graph is tracked.\r\n" +
                               "  - ReferenceHandling.References means that only the root level changes are tracked.\r\n";

                var item = new IllegalEnumerable();
                var exception = Assert.Throws<NotSupportedException>(() => Track.Changes(item));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NotSupportedException>(() => Track.Changes(item, PropertiesSettings.GetOrCreate()));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void WithListOfInts()
            {
                var expected = "Track.Changes(x, y) failed.\r\n" +
                               "The collection type List<int> does not notify changes.\r\n" +
                               "The property With<List<int>>.Value of type List<int> is not supported.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Use a type that implements INotifyCollectionChanged instead of List<int>.\r\n" +
                               "* Use an immutable type instead of List<int>.\r\n" +
                               "  - For immutable types the following must hold:\r\n" +
                               "    - Must be a sealed class or a struct.\r\n" +
                               "    - All fields and properties must be readonly.\r\n" +
                               "    - All field and property types must be immutable.\r\n" +
                               "    - All indexers must be readonly.\r\n" +
                               "    - Event fields are ignored.\r\n" +
                               "* Use PropertiesSettings and specify how change tracking is performed:\r\n" +
                               "  - ReferenceHandling.Structural means that a the entire graph is tracked.\r\n" +
                               "  - ReferenceHandling.References means that only the root level changes are tracked.\r\n" +
                               "  - Exclude a combination of the following:\r\n" +
                               "    - The property With<List<int>>.Value.\r\n" +
                               "    - The type List<int>.\r\n";
                var item = new With<List<int>>();
                var exception = Assert.Throws<NotSupportedException>(() => Track.Changes(item));

                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<NotSupportedException>(() => Track.Changes(item, PropertiesSettings.GetOrCreate()));
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}