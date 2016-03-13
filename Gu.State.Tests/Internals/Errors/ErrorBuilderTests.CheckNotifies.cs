namespace Gu.State.Tests.Internals.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;

    using NUnit.Framework;

    public partial class ErrorBuilderTests
    {
        public class CheckNotifies
        {
            [TestCase(typeof(ObservableCollection<int>))]
            [TestCase(typeof(ErrorTypes.Notifying<int>))]
            public void CheckNotifiesWhenValid(Type type)
            {
                var settings = PropertiesSettings.GetOrCreate();
                var errors = ErrorBuilder.Start()
                                     .CheckNotifies(type, settings)
                                     .Finnish();
                Assert.IsNull(errors);
            }

            [Test]
            public void CheckNotifiesListOfInt()
            {
                var settings = PropertiesSettings.GetOrCreate();
                var type = typeof(List<int>);
                var errors = ErrorBuilder.Start()
                                         .CheckNotifies(type, settings)
                                         .Finnish();
                var expected = new[] { CollectionMustNotifyError.GetOrCreate(type) };
                CollectionAssert.AreEqual(expected, errors.Errors);
                var stringBuilder = new StringBuilder();
                var message = stringBuilder.AppendNotSupported(errors)
                                           .ToString();
                var expectedMessage = "The collection type List<int> does not notify changes.\r\n";
                Assert.AreEqual(expectedMessage, message);

                stringBuilder = new StringBuilder();
                message = stringBuilder.AppendSuggestExclude(errors)
                                       .ToString();
                expectedMessage = "";
                Assert.AreEqual(expectedMessage, message);
            }

            [Test]
            public void CheckNotifiesWithListOfInt()
            {
                Assert.Inconclusive();
                var settings = PropertiesSettings.GetOrCreate();
                var type = typeof(ErrorTypes.With<List<int>>);
                var errors = ErrorBuilder.Start().CheckNotifies(type, settings).Finnish();
                var expected = new Error[] { TypeMustNotifyError.GetOrCreate(type), CollectionMustNotifyError.GetOrCreate(type) };
                CollectionAssert.AreEqual(expected, errors.Errors);
                var stringBuilder = new StringBuilder();
                var message = stringBuilder.AppendNotSupported(errors)
                                           .ToString();
                var expectedMessage = "The type With<List<int>> does not notify changes.\r\n" +
                                      "The type List<int> does not notify changes.\r\n";
                Assert.AreEqual(expectedMessage, message);

                stringBuilder = new StringBuilder();
                message = stringBuilder.AppendSuggestExclude(errors)
                                       .ToString();
                expectedMessage = "  - Exclude a combination of the following:\r\n" +
                                  "    - The property With<List<int>>.Value.\r\n" +
                                  "    - The type List<int>.\r\n";
                Assert.AreEqual(expectedMessage, message);
            }

            [Test]
            public void CheckNotifiesWithInt()
            {
                var settings = PropertiesSettings.GetOrCreate();
                var type = typeof(ErrorTypes.With<int>);
                var errors = ErrorBuilder.Start().CheckNotifies(type, settings).Finnish();
                var expected = new[] { TypeMustNotifyError.GetOrCreate(type) };
                CollectionAssert.AreEqual(expected, errors.Errors);
                var stringBuilder = new StringBuilder();
                var message = stringBuilder.AppendNotSupported(errors)
                                           .ToString();
                var expectedMessage = "The type With<int> does not notify changes.\r\n";
                Assert.AreEqual(expectedMessage, message);

                stringBuilder = new StringBuilder();
                message = stringBuilder.AppendSuggestExclude(errors)
                                       .ToString();
                expectedMessage = "";
                Assert.AreEqual(expectedMessage, message);
            }
        }
    }
}
