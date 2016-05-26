namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class ChangeTrackerTests
    {
        public class Verify
        {
            [Test]
            public void WithIllegal()
            {
                var expected = "Track.VerifyCanTrackChanges(x, y) failed.\r\n" +
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


                var exception = Assert.Throws<NotSupportedException>(() => Track.VerifyCanTrackChanges<WithIllegal>());
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void IllegalEnumerable()
            {
                var expected = "Track.VerifyCanTrackChanges(x, y) failed.\r\n" +
                               "The collection type IllegalEnumerable does not notify changes.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Implement INotifyCollectionChanged for IllegalEnumerable or use a type that does.\r\n" +
                               "* Use PropertiesSettings and specify how change tracking is performed:\r\n" +
                               "  - ReferenceHandling.Structural means that a the entire graph is tracked.\r\n" +
                               "  - ReferenceHandling.References means that only the root level changes are tracked.\r\n";

                var exception = Assert.Throws<NotSupportedException>(() => Track.VerifyCanTrackChanges<IllegalEnumerable>());
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void WithListOfInts()
            {
                var expected = "Track.VerifyCanTrackChanges(x, y) failed.\r\n" +
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
                var exception = Assert.Throws<NotSupportedException>(() => Track.VerifyCanTrackChanges<With<List<int>>>());
                Assert.AreEqual(expected, exception.Message);
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.References)]
            [TestCase(ReferenceHandling.Structural)]
            public void WithSimpleProperties(ReferenceHandling referenceHandling)
            {
                Track.VerifyCanTrackChanges<WithSimpleProperties>(referenceHandling);
                Track.VerifyCanTrackChanges<WithSimpleProperties>(PropertiesSettings.GetOrCreate(referenceHandling));
                Track.VerifyCanTrackChanges(typeof(WithSimpleProperties), PropertiesSettings.GetOrCreate(referenceHandling));
            }
        }
    }
}