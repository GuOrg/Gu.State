namespace Gu.State.Tests.Internals.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using NUnit.Framework;

    using static ErrorTypes;

    public partial class ErrorBuilderTests
    {
        public class CheckReferenceHandling
        {
            [TestCase(typeof(With<int>))]
            [TestCase(typeof(int))]
            public void CheckReferenceHandlingWhenValid(Type type)
            {
                var settings = PropertiesSettings.GetOrCreate();
                var errors = ErrorBuilder.Start()
                                         .CheckRequiresReferenceHandling(type, settings, _ => false)
                                         .Finnish();
                Assert.IsNull(errors);
            }

            [TestCase(true)]
            [TestCase(false)]
            public void CheckReferenceHandlingWhenRootError(bool requiresRef)
            {
                var settings = PropertiesSettings.GetOrCreate(ReferenceHandling.Throw);
                var type = typeof(List<ComplexType>);
                var errors = ErrorBuilder.Start()
                                         .CheckRequiresReferenceHandling(type, settings, _ => requiresRef)
                                         .Finnish();
                if (requiresRef)
                {
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
                else
                {
                    Assert.AreEqual(null, errors);
                }
            }

            [Test]
            public void CheckReferenceHandlingWhenPropertyError()
            {
                var settings = PropertiesSettings.GetOrCreate(ReferenceHandling.Throw);
                var type = typeof(With<List<int>>);
                var errors = ErrorBuilder.Start()
                                         .CheckRequiresReferenceHandling(type, settings, _ => true)
                                         .Finnish();
                var expected = new[] { RequiresReferenceHandling.ComplexType };
                CollectionAssert.AreEqual(expected, errors.Errors);
            }
        }
    }
}
