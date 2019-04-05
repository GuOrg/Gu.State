// ReSharper disable InconsistentNaming
namespace Gu.State.Tests.Internals.Errors
{
    using System.Reflection;
    using NUnit.Framework;

    public class TypeErrorsBuilderTests
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

        [Test]
        public void MergeSame()
        {
            var errors = new TypeErrors(typeof(ErrorTypes.With<int>));
            var merged = ErrorBuilder.Merge(errors, errors);
            Assert.AreSame(merged, errors);
        }

        [Test]
        public void RefLoopAndMustNotify()
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
            var refLoopErrors = ErrorBuilder.Start()
                                     .CreateIfNull(rootType)
                                     .Add(referenceLoop)
                                     .Finnish();
            var notifyErrors = ErrorBuilder.Start()
                                           .CreateIfNull(rootType)
                                           .Add(TypeMustNotifyError.GetOrCreate(rootType))
                                           .Finnish();
            var merged = ErrorBuilder.Merge(refLoopErrors, notifyErrors);
            Assert.AreEqual(2, merged.Errors.Count);
            Assert.AreEqual(8, merged.AllErrors.Count);
        }

        [Test]
        public void RefLoopAndMemberErrors()
        {
            var rootType = typeof(ErrorTypes.With<ErrorTypes.Parent>);
            var valueProperty = rootType.GetProperty(nameof(ErrorTypes.With<ErrorTypes.Parent>.Value), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var parentType = typeof(ErrorTypes.Parent);
            var childProperty = parentType.GetProperty(nameof(ErrorTypes.Parent.Child), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var parentProperty = typeof(ErrorTypes.Child).GetProperty(nameof(ErrorTypes.Child.Parent), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var path = new MemberPath(rootType)
                            .WithProperty(valueProperty);

            var loopPath = path.WithProperty(childProperty)
                               .WithProperty(parentProperty)
                               .WithProperty(childProperty);
            var referenceLoop = new ReferenceLoop(loopPath);
            var refLoopErrors = ErrorBuilder.Start()
                                     .CreateIfNull(rootType)
                                     .Add(referenceLoop)
                                     .Finnish();
            var typeMustNotifyError = TypeMustNotifyError.GetOrCreate(parentType);
            var typeErrors = new TypeErrors(parentType, typeMustNotifyError);
            var memberErrors = new MemberErrors(path, typeErrors);
            var notifyErrors = ErrorBuilder.Start()
                                           .CreateIfNull(rootType)
                                           .Add(memberErrors)
                                           .Finnish();
            var merged = ErrorBuilder.Merge(refLoopErrors, notifyErrors);
            Assert.AreEqual(2, merged.Errors.Count);
            Assert.AreEqual(8, merged.AllErrors.Count);
        }
    }
}
