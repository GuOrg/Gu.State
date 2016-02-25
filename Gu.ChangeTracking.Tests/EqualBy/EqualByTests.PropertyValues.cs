namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Gu.ChangeTracking.Tests.CopyStubs;

    using NUnit.Framework;

    public partial class EqualByTests
    {
        public class PropertyValues
        {
            [TestCaseSource(nameof(EqualsSource))]
            public void PropertyValuesHappyPath(EqualsData data)
            {
                Assert.AreEqual(data.Equals, EqualBy.PropertyValues(data.Source, data.Target));
            }

            [Test]
            public void WithComplexPropertyStructural()
            {
                var x = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType { Name = "c", Value = 2 } };
                var y = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType { Name = "c", Value = 2 } };
                Assert.AreEqual(true, EqualBy.PropertyValues(x, y, ReferenceHandling.Structural));
                x.ComplexType.Value++;
                Assert.AreEqual(false, EqualBy.PropertyValues(x, y, ReferenceHandling.Structural));
            }

            [Test]
            public void WithComplexPropertyReferential()
            {
                var x = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType { Name = "c", Value = 2 } };
                var y = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType { Name = "c", Value = 2 } };
                Assert.AreEqual(false, EqualBy.PropertyValues(x, y, ReferenceHandling.Reference));
                x.ComplexType = y.ComplexType;
                Assert.AreEqual(true, EqualBy.PropertyValues(x, y, ReferenceHandling.Structural));
            }

            [TestCase("1, 2, 3", "1, 2, 3", true)]
            [TestCase("1, 2, 3", "1, 2", false)]
            [TestCase("1, 2", "1, 2, 3", false)]
            [TestCase("5, 2, 3", "1, 2, 3", false)]
            public void EnumarebleStructural(string xs, string ys, bool expected)
            {
                var x = xs.Split(',').Select(int.Parse);
                var y = ys.Split(',').Select(int.Parse);
                Assert.AreEqual(expected, EqualBy.PropertyValues(x, y, ReferenceHandling.Structural));
            }

            [TestCase("1, 2, 3", "1, 2, 3", true)]
            [TestCase("1, 2, 3", "1, 2", false)]
            [TestCase("1, 2", "1, 2, 3", false)]
            [TestCase("5, 2, 3", "1, 2, 3", false)]
            public void ArrayStructural(string xs, string ys, bool expected)
            {
                var x = xs.Split(',').Select(int.Parse).ToArray();
                var y = ys.Split(',').Select(int.Parse).ToArray();
                Assert.AreEqual(expected, EqualBy.PropertyValues(x, y, ReferenceHandling.Structural));
            }

            public static IReadOnlyList<EqualsData> EqualsSource => EqualByTests.EqualsSource;
        }
    }
}
