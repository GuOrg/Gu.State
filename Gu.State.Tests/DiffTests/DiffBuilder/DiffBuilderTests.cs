// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.DiffTests
{
    using NUnit.Framework;

    using static DiffTypes;

    public class DiffBuilderTests
    {
        [Test]
        public void ReturnsSameWhileAlive()
        {
            var x = new ComplexType();
            var y = new ComplexType();
            var structuralSettings = PropertiesSettings.GetOrCreate(ReferenceHandling.Structural);
            using (var t1 = DiffBuilder.GetOrCreate(x, y, structuralSettings))
            {
                using (var t2 = DiffBuilder.GetOrCreate(x, y, structuralSettings))
                {
                    Assert.AreSame(t1, t2);
                    t1.Dispose();
                    t2.Dispose();
                }

                using (var t4 = DiffBuilder.GetOrCreate(x, y, structuralSettings))
                {
                    Assert.AreNotSame(t1, t4);
                }
            }
        }

        [Test]
        public void ReturnsDifferentForDifferentSettings()
        {
            var x = new ComplexType();
            var y = new ComplexType();
            using (var t1 = DiffBuilder.GetOrCreate(x, y, PropertiesSettings.GetOrCreate(ReferenceHandling.Structural)))
            {
                using (var t2 = DiffBuilder.GetOrCreate(x, y, PropertiesSettings.GetOrCreate(ReferenceHandling.Throw)))
                {
                    Assert.AreNotSame(t1, t2);
                }
            }
        }

        [Test]
        public void ReturnsDifferentForDifferentPairs()
        {
            var x = new ComplexType();
            var y = new ComplexType();
            var structuralSettings = PropertiesSettings.GetOrCreate(ReferenceHandling.Structural);
            using (var t1 = DiffBuilder.GetOrCreate(x, y, structuralSettings))
            {
                using (var t2 = DiffBuilder.GetOrCreate(y, x, structuralSettings))
                {
                    Assert.AreNotSame(t1, t2);
                }
            }
        }
    }
}
