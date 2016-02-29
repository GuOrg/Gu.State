namespace Gu.ChangeTracking.Tests.EqualByTests.FieldValues
{
    using System.Collections.Generic;
    using System.Linq;

    using Gu.ChangeTracking.Tests.EqualByTests;

    using NUnit.Framework;

    public  class Classes_
    {
        public class FieldValues
        {
            [TestCase("1, 2, 3", "1, 2, 3", true)]
            [TestCase("1, 2, 3", "1, 2", false)]
            [TestCase("1, 2", "1, 2, 3", false)]
            [TestCase("5, 2, 3", "1, 2, 3", false)]
            public void ArrayFieldValuesStructural(string xs, string ys, bool expected)
            {
                var x = xs.Split(',').Select(int.Parse).ToArray();
                var y = ys.Split(',').Select(int.Parse).ToArray();
                Assert.AreEqual(expected, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
            }

            [Test]
            public void FieldValuesStructural()
            {
                var x = new EqualByTypes.WithComplexValue { Name = "a", Value = 1, ComplexValue = new EqualByTypes.ComplexType { Name = "c", Value = 2 } };
                var y = new EqualByTypes.WithComplexValue { Name = "a", Value = 1, ComplexValue = new EqualByTypes.ComplexType { Name = "c", Value = 2 } };
                Assert.AreEqual(true, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
                x.ComplexValue.Value++;
                Assert.AreEqual(false, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
            }

            [Test]
            public void FieldValuesReferential()
            {
                var x = new EqualByTypes.WithComplexValue { Name = "a", Value = 1, ComplexValue = new EqualByTypes.ComplexType { Name = "c", Value = 2 } };
                var y = new EqualByTypes.WithComplexValue { Name = "a", Value = 1, ComplexValue = new EqualByTypes.ComplexType { Name = "c", Value = 2 } };
                Assert.AreEqual(false, EqualBy.FieldValues(x, y, ReferenceHandling.References));
                x.ComplexValue = y.ComplexValue;
                Assert.AreEqual(true, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
            }

            [TestCase("1, 2, 3", "1, 2, 3", true)]
            [TestCase("1, 2, 3", "1, 2", false)]
            [TestCase("1, 2", "1, 2, 3", false)]
            [TestCase("5, 2, 3", "1, 2, 3", false)]
            public void EnumarebleStructural(string xs, string ys, bool expected)
            {
                var x = xs.Split(',').Select(int.Parse);
                var y = ys.Split(',').Select(int.Parse);
                Assert.AreEqual(expected, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
            }

            [TestCase(0, 0, 0, 0, true)]
            [TestCase(0, 1, 0, 1, true)]
            [TestCase(1, 1, 1, 1, true)]
            [TestCase(0, 2, 0, 1, false)]
            [TestCase(0, 1, 0, 2, false)]
            [TestCase(1, 1, 0, 1, false)]
            [TestCase(0, 1, 1, 1, false)]
            public void EnumarebleRepeatStructural(int startX, int countX, int startY, int countY, bool expected)
            {
                var x = Enumerable.Repeat(startX, countX);
                var y = Enumerable.Repeat(startY, countY);
                Assert.AreEqual(expected, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
            }

            [Test]
            public void EnumarebleNullsStructural()
            {
                var x = new object[] { 1, null }.Select(z => z);
                var y = new object[] { 1, null }.Select(z => z);
                Assert.AreEqual(true, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));

                x = new object[] { 1 }.Select(z => z);
                y = new object[] { 1, null }.Select(z => z);
                Assert.AreEqual(false, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));

                x = new object[] { 1, null }.Select(z => z);
                y = new object[] { 1 }.Select(z => z);
                Assert.AreEqual(false, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
            }
        }
    }
}