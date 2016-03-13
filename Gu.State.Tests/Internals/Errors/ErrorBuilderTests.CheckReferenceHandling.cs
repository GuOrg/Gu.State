namespace Gu.State.Tests.Internals.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using NUnit.Framework;

    public partial class ErrorBuilderTests
    {
        public class CheckReferenceHandling
        {
            [TestCase(typeof(ErrorTypes.With<int>))]
            [TestCase(typeof(int))]
            public void CheckReferenceHandlingWhenValid(Type type)
            {
                var settings = PropertiesSettings.GetOrCreate();
                var errors = ErrorBuilder.Start()
                                         .CheckReferenceHandling(type, settings, _ => false)
                                         .Finnish();
                Assert.IsNull(errors);
            }

            [TestCase(true)]
            [TestCase(false)]
            public void CheckReferenceHandlingWhenRootError(bool requiresRef)
            {
                var settings = PropertiesSettings.GetOrCreate();
                var type = typeof(List<int>);
                var errors = ErrorBuilder.Start()
                                         .CheckReferenceHandling(type, settings, _ => requiresRef)
                                         .Finnish();
                var error = RequiresReferenceHandling.Enumerable;
                var expected = new[] { error };
                CollectionAssert.AreEqual(expected, errors.Errors);
                var stringBuilder = new StringBuilder();
                var message = stringBuilder.AppendNotSupported(errors)
                                           .ToString();
                var expectedMessage = "";
                Assert.AreEqual(expectedMessage, message);

                stringBuilder = new StringBuilder();
                message = stringBuilder.AppendSuggestExclude(errors)
                                       .ToString();
                expectedMessage = "";
                Assert.AreEqual(expectedMessage, message);
            }

            [Test]
            public void CheckReferenceHandlingWhenPropertyError()
            {
                var settings = PropertiesSettings.GetOrCreate();
                var type = typeof(ErrorTypes.With<List<int>>);
                var errors = ErrorBuilder.Start()
                                         .CheckReferenceHandling(type, settings, _ => true)
                                         .Finnish();
                var expected = new[] { RequiresReferenceHandling.Other };
                CollectionAssert.AreEqual(expected, errors.Errors);
            }
        }
    }
}
