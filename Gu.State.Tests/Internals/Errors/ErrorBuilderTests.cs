namespace Gu.State.Tests.Internals.Errors
{
    using NUnit.Framework;

    public partial class TypeErrorsBuilderTests
    {
        [Test]
        public void FinnishWhenNull()
        {
            var errors = ErrorBuilder.Start()
                                     .Finnish();
            Assert.IsNull(errors);
        }

        [Test]
        public void FinnishAndNoErrors()
        {
            var errors = ErrorBuilder.Start()
                                     .CreateIfNull(typeof(ErrorTypes.With<int>))
                                     .Finnish();
            Assert.IsNull(errors);
        }

        [Test]
        public void FinnishWithError()
        {
            var type = typeof(ErrorTypes.With<int>);
            var errors = ErrorBuilder.Start()
                                      .CreateIfNull(type)
                                      .Add(TypeMustNotifyError.GetOrCreate(type))
                                      .Finnish();
            Assert.AreEqual(type, errors.Type);
            CollectionAssert.AreEqual(new[] { TypeMustNotifyError.GetOrCreate(type) }, errors.Errors);
        }
    }
}
