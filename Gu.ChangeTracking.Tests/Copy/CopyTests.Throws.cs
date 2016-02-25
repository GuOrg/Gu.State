namespace Gu.ChangeTracking.Tests
{
    using System;

    using NUnit.Framework;

    public partial class CopyTests
    {
        public class Throws
        {
            [Test]
            public void PropertyValuesWithoutDefaultCtor()
            {
                var x = new WithoutDefaultCtorManager(new WithoutDefaultCtor(1));
                var y = new WithoutDefaultCtorManager(null);
                var exception = Assert.Throws<NotSupportedException>(() => Copy.PropertyValues(x, y, ReferenceHandling.Structural));
                var expected = "Activator.CreateInstance failed for type WithoutDefaultCtor.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Add a parameterless constructor to WithoutDefaultCtor\r\n" +
                               "* Make WithoutDefaultCtor immutable or use an immutable type. For immutable types the following must hold:\r\n" +
                               "  - Must be a sealed class or a struct.\r\n" +
                               "  - All fields and properties must be readonly.\r\n" +
                               "  - All field and property types must be immutable.\r\n" +
                               "  - All indexers must be readonly.\r\n" +
                               "  - Event fields are ignored.\r\n" +
                               "* Provide CopySettings and specify either or both of:\r\n" +
                               "  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Reference)}\r\n" +
                               "  - Exclude the member or type if feasible\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void FieldValuesWithoutDefaultCtor()
            {
                var x = new WithoutDefaultCtorManager(new WithoutDefaultCtor(1));
                var y = new WithoutDefaultCtorManager(null);
                var exception = Assert.Throws<NotSupportedException>(() => Copy.FieldValues(x, y, ReferenceHandling.Structural));
                var expected = "Activator.CreateInstance failed for type WithoutDefaultCtor.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Add a parameterless constructor to WithoutDefaultCtor\r\n" +
                               "* Make WithoutDefaultCtor immutable or use an immutable type. For immutable types the following must hold:\r\n" +
                               "  - Must be a sealed class or a struct.\r\n" +
                               "  - All fields and properties must be readonly.\r\n" +
                               "  - All field and property types must be immutable.\r\n" +
                               "  - All indexers must be readonly.\r\n" +
                               "  - Event fields are ignored.\r\n" +
                               "* Provide CopySettings and specify either or both of:\r\n" +
                               "  - {typeof(ReferenceHandling).Name}.{nameof(ReferenceHandling.Reference)}\r\n" +
                               "  - Exclude the member or type if feasible\r\n";
                Assert.AreEqual(expected, exception.Message);
            }

            public class WithoutDefaultCtorManager
            {
                private WithoutDefaultCtor withoutDefaultCtor;

                public WithoutDefaultCtorManager(WithoutDefaultCtor withoutDefaultCtor)
                {
                    this.withoutDefaultCtor = withoutDefaultCtor;
                }

                public WithoutDefaultCtor WithoutDefaultCtor
                {
                    get
                    {
                        return this.withoutDefaultCtor;
                    }
                    set
                    {
                        this.withoutDefaultCtor = value;
                    }
                }
            }

            public class WithoutDefaultCtor
            {
                private int value;

                public WithoutDefaultCtor(int value)
                {
                    this.value = value;
                }

                public int Value
                {
                    get
                    {
                        return this.value;
                    }
                    set
                    {
                        this.value = value;
                    }
                }
            }
        }
    }
}
