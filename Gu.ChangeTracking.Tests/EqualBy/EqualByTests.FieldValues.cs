namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gu.ChangeTracking.Tests.CopyStubs;

    using NUnit.Framework;

    public partial class EqualByTests
    {
        public class FieldValues
        {
            [TestCaseSource(nameof(EqualsSource))]
            public void FieldValuesHappyPath(EqualsData data)
            {
                Assert.AreEqual(data.Equals, EqualBy.FieldValues(data.Source, data.Target));
            }

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
                var x = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType { Name = "c", Value = 2 } };
                var y = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType { Name = "c", Value = 2 } };
                Assert.AreEqual(true, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
                x.ComplexType.Value++;
                Assert.AreEqual(false, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
            }

            [Test]
            public void FieldValuesReferential()
            {
                var x = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType { Name = "c", Value = 2 } };
                var y = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType { Name = "c", Value = 2 } };
                Assert.AreEqual(false, EqualBy.FieldValues(x, y, ReferenceHandling.Reference));
                x.ComplexType = y.ComplexType;
                Assert.AreEqual(true, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
            }

        }
    }
}