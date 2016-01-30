namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Gu.ChangeTracking.Tests.ChangeTrackerStubs;
    using Gu.ChangeTracking.Tests.CopyStubs;
    using NUnit.Framework;

    [SuppressMessage("ReSharper", "NotResolvedInText")]
    public class SynchronizerTests
    {
        [Test]
        public void SynchronizerHappyPath()
        {
            var source = new WithSimpleProperties
            {
                IntValue = 1,
                NullableIntValue = 2,
                StringValue = "3",
                EnumValue = StringSplitOptions.RemoveEmptyEntries
            };
            var target = new WithSimpleProperties { IntValue = 3, NullableIntValue = 4 };
            using (PropertySynchronizer.Create(source, target))
            {
                Assert.AreEqual(1, source.IntValue);
                Assert.AreEqual(1, target.IntValue);
                Assert.AreEqual(2, source.NullableIntValue);
                Assert.AreEqual(2, target.NullableIntValue);
                Assert.AreEqual("3", source.StringValue);
                Assert.AreEqual("3", target.StringValue);
                Assert.AreEqual(StringSplitOptions.RemoveEmptyEntries, source.EnumValue);
                Assert.AreEqual(StringSplitOptions.RemoveEmptyEntries, target.EnumValue);

                source.IntValue = 5;
                Assert.AreEqual(5, source.IntValue);
                Assert.AreEqual(5, target.IntValue);
            }

            source.IntValue = 6;
            Assert.AreEqual(6, source.IntValue);
            Assert.AreEqual(5, target.IntValue);
        }

        [Test]
        public void SynchronizerWithCalculated()
        {
            var source = new WithCalculatedProperty { Value = 1 };
            var target = new WithCalculatedProperty { Value = 3 };
            using (PropertySynchronizer.Create(source, target))
            {
                Assert.AreEqual(1, source.Value);
                Assert.AreEqual(1, target.Value);

                source.Value = 5;
                Assert.AreEqual(5, source.Value);
                Assert.AreEqual(5, target.Value);
            }

            source.Value = 6;
            Assert.AreEqual(6, source.Value);
            Assert.AreEqual(5, target.Value);
        }

        [Test]
        public void SynchronizerExcludes()
        {
            var source = new BaseClass { Value = 1, Excluded = 2 };
            var target = new BaseClass { Value = 3, Excluded = 4 };
            using (PropertySynchronizer.Create(source, target, nameof(BaseClass.Excluded)))
            {
                Assert.AreEqual(1, source.Value);
                Assert.AreEqual(1, target.Value);
                Assert.AreEqual(2, source.Excluded);
                Assert.AreEqual(4, target.Excluded);

                source.Value = 5;
                Assert.AreEqual(5, source.Value);
                Assert.AreEqual(5, target.Value);

                source.Excluded = 7;
                Assert.AreEqual(7, source.Excluded);
                Assert.AreEqual(4, target.Excluded);
            }

            source.Value = 6;
            Assert.AreEqual(6, source.Value);
            Assert.AreEqual(5, target.Value);
        }

        [Test]
        public void SynchronizerHandlesMissingProperty()
        {
            var source = new BaseClass { Value = 1, Excluded = 2 };
            var target = new BaseClass { Value = 3, Excluded = 4 };
            using (PropertySynchronizer.Create(source, target))
            {
                Assert.AreEqual(1, source.Value);
                Assert.AreEqual(1, target.Value);
                Assert.AreEqual(2, source.Excluded);
                Assert.AreEqual(2, target.Excluded);
                source.OnPropertyChanged("Missing");
                source.Value = 5;
                Assert.AreEqual(5, source.Value);
                Assert.AreEqual(5, target.Value);
            }

            source.Value = 6;
            Assert.AreEqual(6, source.Value);
            Assert.AreEqual(5, target.Value);
        }

        [TestCase(null)]
        [TestCase("")]
        public void SynchronizerUpdatesAll(string prop)
        {
            var source = new BaseClass { Value = 1, Excluded = 2 };
            var target = new BaseClass { Value = 3, Excluded = 4 };
            using (PropertySynchronizer.Create(source, target))
            {
                Assert.AreEqual(1, source.Value);
                Assert.AreEqual(1, target.Value);
                Assert.AreEqual(2, source.Excluded);
                Assert.AreEqual(2, target.Excluded);
                source.SetFields(5, 6);
                source.OnPropertyChanged(prop);
                Assert.AreEqual(5, source.Value);
                Assert.AreEqual(5, target.Value);
                Assert.AreEqual(6, source.Excluded);
                Assert.AreEqual(6, target.Excluded);
            }

            source.Value = 6;
            Assert.AreEqual(6, source.Value);
            Assert.AreEqual(5, target.Value);
        }
    }
}
