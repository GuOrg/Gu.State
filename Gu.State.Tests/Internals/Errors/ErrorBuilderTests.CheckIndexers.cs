namespace Gu.State.Tests.Internals.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    public static partial class ErrorBuilderTests
    {
        public static class CheckIndexers
        {
            [TestCase(typeof(ErrorTypes.With<int>))]
            [TestCase(typeof(List<int>))]
            public static void CheckIndexersWhenValid(Type type)
            {
                var settings = PropertiesSettings.GetOrCreate();
                var errors = ErrorBuilder.Start()
                                         .CheckIndexers(type, settings)
                                         .Finnish();
                Assert.IsNull(errors);
            }

            [Test]
            public static void CheckIndexersWhenError()
            {
                var settings = PropertiesSettings.GetOrCreate();
                var type = typeof(ErrorTypes.WithIndexer);
                var errors = ErrorBuilder.Start()
                                     .CheckIndexers(type, settings)
                                     .Finnish();
                var indexer = type.GetProperties().Single();
                var error = UnsupportedIndexer.GetOrCreate(indexer);
                var expected = new[] { error };
                CollectionAssert.AreEqual(expected, errors.Errors);
                var stringBuilder = new StringBuilder();
                var message = stringBuilder.AppendNotSupported(errors)
                                     .ToString();
                var expectedMessage = "Indexers are not supported.\r\n" +
                                      "  - The property WithIndexer[int] is an indexer and not supported.\r\n";
                Assert.AreEqual(expectedMessage, message);

                stringBuilder = new StringBuilder();
                message = stringBuilder.AppendSuggestExclude(errors)
                                       .ToString();
                expectedMessage = "  - Exclude a combination of the following:\r\n" +
                                  "    - The indexer property WithIndexer[int].\r\n";
                Assert.AreEqual(expectedMessage, message);
            }
        }
    }
}
