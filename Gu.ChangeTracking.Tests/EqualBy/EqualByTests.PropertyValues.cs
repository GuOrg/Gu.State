namespace Gu.ChangeTracking.Tests
{
    using System.Collections;
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

            [TestCase(0, 0, 0, 0, true)]
            [TestCase(0, 1, 0, 1, true)]
            [TestCase(1, 1, 1, 1, true)]
            [TestCase(0, 2, 0, 1, false)]
            [TestCase(0, 1, 0, 2, false)]
            [TestCase(1, 1, 0, 1, false)]
            [TestCase(0, 1, 1, 1, false)]
            public void EnumarebleRepeatStructural1(int startX, int countX, int startY, int countY, bool expected)
            {
                var x = Enumerable.Repeat(startX, countX);
                var y = Enumerable.Repeat(startY, countY);
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
