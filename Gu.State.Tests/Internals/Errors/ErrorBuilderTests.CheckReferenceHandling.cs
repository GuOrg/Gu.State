namespace Gu.State.Tests.Internals.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using NUnit.Framework;

    using static ErrorTypes;

    public static partial class ErrorBuilderTests
    {
        public static class CheckReferenceHandling
        {
            [TestCase(typeof(With<int>))]
            [TestCase(typeof(int))]
            public static void CheckReferenceHandlingWhenValid(Type type)
            {
                var settings = PropertiesSettings.GetOrCreate();
                var errors = ErrorBuilder.Start()
                                         .CheckRequiresReferenceHandling(type, settings, _ => false)
                                         .Finnish();
                Assert.IsNull(errors);
            }

            [TestCase(true)]
            [TestCase(false)]
            public static void CheckReferenceHandlingWhenRootError(bool requiresRef)
            {
                var settings = PropertiesSettings.GetOrCreate(ReferenceHandling.Throw);
                var type = typeof(List<ComplexType>);
                var errors = ErrorBuilder.Start()
                                         .CheckRequiresReferenceHandling(type, settings, _ => requiresRef)
                                         .Finnish();
                if (requiresRef)
                {
                    var error = RequiresReferenceHandling.Default;
                    var expected = new[] { error };
                    CollectionAssert.AreEqual(expected, errors.Errors);
                    var stringBuilder = new StringBuilder();
                    var message = stringBuilder.AppendNotSupported(errors)
                                               .ToString();
                    var expectedMessage = string.Empty;
                    Assert.AreEqual(expectedMessage, message);

                    stringBuilder = new StringBuilder();
                    message = stringBuilder.AppendSuggestExclude(errors)
                                           .ToString();
                    expectedMessage = string.Empty;
                    Assert.AreEqual(expectedMessage, message);
                }
                else
                {
                    Assert.AreEqual(null, errors);
                }
            }

            [Test]
            public static void CheckReferenceHandlingWhenPropertyError()
            {
                var settings = PropertiesSettings.GetOrCreate(ReferenceHandling.Throw);
                var type = typeof(With<List<int>>);
                var errors = ErrorBuilder.Start()
                                         .CheckRequiresReferenceHandling(type, settings, _ => true)
                                         .Finnish();
                var expected = new[] { RequiresReferenceHandling.Default };
                CollectionAssert.AreEqual(expected, errors.Errors);
            }
        }
    }
}
