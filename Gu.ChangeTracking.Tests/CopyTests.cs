namespace Gu.ChangeTracking.Tests
{
    using Gu.ChangeTracking.Tests.Helpers;
    using NUnit.Framework;

    public class CopyTests
    {
        [Test]
        public void VerifyCanCopyFieldValuesHappyPath()
        {
            Copy.VerifyCanCopyFieldValues<BaseClass>();
        }

        [Test]
        public void FieldValuesHappyPath()
        {
            var source = new BaseClass { Value = 1, Excluded = 2 };
            var target = new BaseClass { Value = 3, Excluded = 4 };
            Copy.FieldValues(source, target);
            Assert.AreEqual(1, source.Value);
            Assert.AreEqual(1, target.Value);
            Assert.AreEqual(2, source.Excluded);
            Assert.AreEqual(2, target.Excluded);
        }

        [Test]
        public void FieldValuesIgnores()
        {
            var source = new BaseClass { Value = 1, Excluded = 2 };
            var target = new BaseClass { Value = 3, Excluded = 4 };
            Copy.FieldValues(source, target, "excluded");
            Assert.AreEqual(1, source.Value);
            Assert.AreEqual(1, target.Value);
            Assert.AreEqual(2, source.Excluded);
            Assert.AreEqual(4, target.Excluded);
        }

        [Test]
        public void VerifyCanCopyPropertyValuesHappyPath()
        {
            Copy.VerifyCanCopyPropertyValues<BaseClass>();
        }

        [Test]
        public void PropertyValuesHappyPath()
        {
            var source = new BaseClass { Value = 1, Excluded = 2 };
            var target = new BaseClass { Value = 3, Excluded = 4 };
            Copy.PropertyValues(source, target);
            Assert.AreEqual(1, source.Value);
            Assert.AreEqual(1, target.Value);
            Assert.AreEqual(2, source.Excluded);
            Assert.AreEqual(2, target.Excluded);
        }

        [Test]
        public void PropertyValuesIgnores()
        {
            var source = new BaseClass { Value = 1, Excluded = 2 };
            var target = new BaseClass { Value = 3, Excluded = 4 };
            Copy.PropertyValues(source, target, nameof(BaseClass.Excluded));
            Assert.AreEqual(1, source.Value);
            Assert.AreEqual(1, target.Value);
            Assert.AreEqual(2, source.Excluded);
            Assert.AreEqual(4, target.Excluded);
        }
    }
}