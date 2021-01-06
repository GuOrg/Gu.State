// ReSharper disable InconsistentNaming
namespace Gu.State.Tests.Internals.Errors
{
    using System.Reflection;
    using System.Text;
    using NUnit.Framework;

    public static partial class ErrorBuilderTests
    {
        public static class WithReferenceLoop
        {
            [Test]
            public static void WhenError()
            {
                var rootType = typeof(ErrorTypes.With<ErrorTypes.Parent>);
                var valueProperty = rootType.GetProperty(nameof(ErrorTypes.With<ErrorTypes.Parent>.Value), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                var childProperty = typeof(ErrorTypes.Parent).GetProperty(nameof(ErrorTypes.Parent.Child), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                var parentProperty = typeof(ErrorTypes.Child).GetProperty(nameof(ErrorTypes.Child.Parent), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                var path = new MemberPath(rootType)
                                .WithProperty(valueProperty)
                                .WithProperty(childProperty)
                                .WithProperty(parentProperty)
                                .WithProperty(childProperty);
                var referenceLoop = new ReferenceLoop(path);
                var typeErrors = ErrorBuilder.Start()
                                         .CreateIfNull(rootType)
                                         .Add(referenceLoop)
                                         .Finnish();
                var expected = "The property Parent.Child of type Child is in a reference loop.\r\n" +
                               "  - The loop is With<Parent>.Value.Child.Parent.Child...\r\n" +
                               "The property With<Parent>.Value of type Parent is not supported.\r\n" +
                               "The property Parent.Child of type Child is not supported.\r\n" +
                               "The property Child.Parent of type Parent is not supported.\r\n";
                var actual = new StringBuilder().AppendNotSupported(typeErrors).ToString();
                Assert.AreEqual(expected, actual);

                Assert.AreEqual(1, typeErrors.Errors.Count);
                Assert.AreEqual(7, typeErrors.AllErrors.Count);
            }
        }
    }
}
