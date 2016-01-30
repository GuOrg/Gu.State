namespace Gu.ChangeTracking.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using Gu.ChangeTracking.Tests.Helpers;
    using NUnit.Framework;

    [SuppressMessage("ReSharper", "NotResolvedInText")]
    public class SynchronizerTests
    {
        [Test]
        public void SynchronizerHappyPath()
        {
            var source = new BaseClass { Value = 1, Excluded = 2 };
            var target = new BaseClass { Value = 3, Excluded = 4 };
            using (PropertySynchronizer.Create(source, target))
            {
                Assert.AreEqual(1, source.Value);
                Assert.AreEqual(1, target.Value);
                Assert.AreEqual(2, source.Excluded);
                Assert.AreEqual(2, target.Excluded);

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
