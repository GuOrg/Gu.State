namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Collections.Generic;

    using Gu.ChangeTracking.Tests.CopyStubs;

    using NUnit.Framework;

    public class EqualByTests
    {
        [TestCaseSource(nameof(EqualsSource))]
        public void FieldValuesHappyPath(EqualsData data)
        {
            Assert.AreEqual(data.Equals, EqualBy.FieldValues(data.Source, data.Target));
        }

        [TestCaseSource(nameof(EqualsSource))]
        public void PropertyValuesHappyPath(EqualsData data)
        {
            Assert.AreEqual(data.Equals, EqualBy.PropertyValues(data.Source, data.Target));
        }

        public static IReadOnlyList<EqualsData> EqualsSource = new List<EqualsData>
        {
            new EqualsData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           true),
            new EqualsData(new WithSimpleProperties(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleProperties(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           true),
            new EqualsData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleProperties(5, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleProperties(1, 5, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleProperties(1, null, "3", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleProperties(1, 2, "5", StringSplitOptions.RemoveEmptyEntries),
                           false),
            new EqualsData(new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries),
                           new WithSimpleProperties(1, 2, "3", StringSplitOptions.None),
                           false),
        };

        public class EqualsData
        {
            public EqualsData(object source, object target, bool @equals)
            {
                this.Source = source;
                this.Target = target;
                this.Equals = @equals;
            }

            public object Source { get; }

            public object Target { get; }

            public bool Equals { get; }
        }
    }
}