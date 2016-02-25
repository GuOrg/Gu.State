namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Gu.ChangeTracking.Tests.ChangeTrackerStubs;
    using NUnit.Framework;

    public partial class ChangeTrackerTests
    {
        public class Throws
        {
            [Test]
            public void AddIllegalThrows()
            {
                var expected = "Create ChangeTracker failed for item: ObservableCollection<ComplexType>[0].Illegal.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Implement INotifyPropertyChanged for IllegalType or use a type that notifies.\r\n" +
                               "* Use an immutable type instead of IllegalType. For immutable types the following must hold:\r\n" +
                               "  - Must be a sealed class or a struct.\r\n" +
                               "  - All fields and properties must be readonly.\r\n" +
                               "  - All field and property types must be immutable.\r\n" +
                               "  - All indexers must be readonly.\r\n" +
                               "  - Event fields are ignored.\r\n" +
                               "* Use ChangeTrackerSettings and add a specialcase for IllegalType example:\r\n" +
                               "    settings.AddIgnoredType<IllegalType>()\r\n" +
                               "    or:\r\n" +
                               "    settings.AddIgnoredProperty(typeof(IllegalSubType).GetProperty(nameof(IllegalSubType.Illegal)))\r\n" +
                               "    Note that this means that the ChangeTracker does not track changes so you are responsible for any tracking needed.\r\n";

                var root = new ObservableCollection<ComplexType>();
                using (ChangeTracker.Track(root, ChangeTrackerSettings.Default))
                {
                    var exception = Assert.Throws<NotSupportedException>(() => root.Add(new IllegalSubType()));
                    Assert.AreEqual(expected, exception.Message);
                }
            }

            [Test]
            public void SetIllegalThrows()
            {
                var expected = "Create ChangeTracker failed for property: With<ComplexType>.Value.Illegal.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Implement INotifyPropertyChanged for IllegalType or use a type that notifies.\r\n" +
                               "* Use an immutable type instead of IllegalType. For immutable types the following must hold:\r\n" +
                               "  - Must be a sealed class or a struct.\r\n" +
                               "  - All fields and properties must be readonly.\r\n" +
                               "  - All field and property types must be immutable.\r\n" +
                               "  - All indexers must be readonly.\r\n" +
                               "  - Event fields are ignored.\r\n" +
                               "* Use ChangeTrackerSettings and add a specialcase for IllegalType example:\r\n" +
                               "    settings.AddIgnoredType<IllegalType>()\r\n" +
                               "    or:\r\n" +
                               "    settings.AddIgnoredProperty(typeof(IllegalSubType).GetProperty(nameof(IllegalSubType.Illegal)))\r\n" +
                               "    Note that this means that the ChangeTracker does not track changes so you are responsible for any tracking needed.\r\n";

                var root = new With<ComplexType>();
                using (ChangeTracker.Track(root, ChangeTrackerSettings.Default))
                {
                    var exception = Assert.Throws<NotSupportedException>(() => root.Value = new IllegalSubType());
                    Assert.AreEqual(expected, exception.Message);
                }
            }

            [Test]
            public void WIthIllegalObject()
            {
                var item = new WithIllegal();
                var expected = "Create ChangeTracker failed for property: WithIllegal.Illegal.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Implement INotifyPropertyChanged for IllegalType or use a type that notifies.\r\n" +
                               "* Use an immutable type instead of IllegalType. For immutable types the following must hold:\r\n" +
                               "  - Must be a sealed class or a struct.\r\n" +
                               "  - All fields and properties must be readonly.\r\n" +
                               "  - All field and property types must be immutable.\r\n" +
                               "  - All indexers must be readonly.\r\n" +
                               "  - Event fields are ignored.\r\n" +
                               "* Use ChangeTrackerSettings and add a specialcase for IllegalType example:\r\n" +
                               "    settings.AddIgnoredType<IllegalType>()\r\n" +
                               "    or:\r\n" +
                               "    settings.AddIgnoredProperty(typeof(WithIllegal).GetProperty(nameof(WithIllegal.Illegal)))\r\n" +
                               "    Note that this means that the ChangeTracker does not track changes so you are responsible for any tracking needed.\r\n";
                var exception = Assert.Throws<NotSupportedException>(() => ChangeTracker.Track(item, ChangeTrackerSettings.Default));
                Assert.AreEqual(expected, exception.Message);

                exception = Assert.Throws<NotSupportedException>(() => ChangeTracker.Track(item));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void OnIllegalEnumerable()
            {
                var item = new IllegalEnumerable();
                var exception = Assert.Throws<NotSupportedException>(() => ChangeTracker.Track(item));
                var expected = "Create ChangeTracker failed for type: IllegalEnumerable.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Use ObservableCollection<T> or another collection type that notifies instead of IllegalEnumerable.\r\n" +
                               "* Make IllegalEnumerable implement the interfaces INotifyCollectionChanged and IList.\r\n" +
                               "* Make IllegalEnumerable immutable or use an immutable type. For immutable types the following must hold:\r\n" +
                               "  - Must be a sealed class or a struct.\r\n" +
                               "  - All fields and properties must be readonly.\r\n" +
                               "  - All field and property types must be immutable.\r\n" +
                               "  - All indexers must be readonly.\r\n" +
                               "  - Event fields are ignored.\r\n" +
                               "* Use ChangeTrackerSettings and add a specialcase for IllegalEnumerable example:\r\n" +
                               "    settings.AddIgnoredType<IllegalEnumerable>()\r\n" +
                               "    Note that this means that the ChangeTracker does not track changes so you are responsible for any tracking needed.\r\n";
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<NotSupportedException>(() => ChangeTracker.Track(item, ChangeTrackerSettings.Default));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void WithListOfInts()
            {
                var item = new With<List<int>>();
                var exception = Assert.Throws<NotSupportedException>(() => ChangeTracker.Track(item));
                var expected = "Create ChangeTracker failed for property: With<List<int>>.Value.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Use ObservableCollection<T> or another collection type that notifies instead of List<int>.\r\n" +
                               "* Use an immutable type instead of List<int>. For immutable types the following must hold:\r\n" +
                               "  - Must be a sealed class or a struct.\r\n" +
                               "  - All fields and properties must be readonly.\r\n" +
                               "  - All field and property types must be immutable.\r\n" +
                               "  - All indexers must be readonly.\r\n" +
                               "  - Event fields are ignored.\r\n" +
                               "* Use ChangeTrackerSettings and add a specialcase for List<int> example:\r\n" +
                               "    settings.AddIgnoredType<List<int>>()\r\n" +
                               "    or:\r\n" +
                               "    settings.AddIgnoredProperty(typeof(With<List<int>>).GetProperty(nameof(With<List<int>>.Value)))\r\n" +
                               "    Note that this means that the ChangeTracker does not track changes so you are responsible for any tracking needed.\r\n";
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<NotSupportedException>(() => ChangeTracker.Track(item, ChangeTrackerSettings.Default));
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}