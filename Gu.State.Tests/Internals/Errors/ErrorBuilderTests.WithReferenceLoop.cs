namespace Gu.State.Tests.Internals.Errors
{
    using System.Text;
    using NUnit.Framework;

    public partial class ErrorBuilderTests
    {
        public class WithReferenceLoop
        {
            [Test]
            public void WhenError()
            {
                var rootType = typeof(ErrorTypes.With<ErrorTypes.Parent>);
                var valueProperty = rootType.GetProperty(nameof(ErrorTypes.With<ErrorTypes.Parent>.Value));
                var childProperty = typeof(ErrorTypes.Parent).GetProperty(nameof(ErrorTypes.Parent.Child));
                var parentProperty = typeof(ErrorTypes.Child).GetProperty(nameof(ErrorTypes.Child.Parent));
                var path = new MemberPath(rootType)
                                .WithProperty(valueProperty)
                                .WithProperty(childProperty)
                                .WithProperty(parentProperty)
                                .WithProperty(childProperty);
                var referenceLoop = new ReferenceLoop(path);
                var errors = ErrorBuilder.Start()
                                         .CreateIfNull(rootType)
                                         .Add(referenceLoop)
                                         .Finnish();
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendNotSupported(errors);
                var expected = "The property Parent.Child of type Child is in a reference loop.\r\n" +
                               "  - The loop is With<Parent>.Value.Child.Parent.Child...\r\n" +
                               "The property With<Parent>.Value of type Parent is not supported.\r\n" +
                               "The property Parent.Child of type Child is not supported.\r\n" +
                               "The property Child.Parent of type Parent is not supported.\r\n";
                var actual = errorBuilder.ToString();
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
