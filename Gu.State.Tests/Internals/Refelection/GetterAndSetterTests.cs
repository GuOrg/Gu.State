namespace Gu.State.Tests.Internals.Refelection
{
    using NUnit.Framework;

    using static TypeExtTypes;

    public class GetterAndSetterTests
    {
        [Test]
        public void CreateFromPropertyInfo()
        {
            var propertyInfo = typeof(ComplexType).GetProperty(nameof(ComplexType.Value));
            var getterAndSetter = (GetterAndSetter<ComplexType, int>)GetterAndSetter.Create(propertyInfo);
            var complexType = new ComplexType();
            getterAndSetter.SetValue(complexType, 1);
            Assert.AreEqual(1, complexType.Value);
            Assert.AreEqual(1, getterAndSetter.GetValue(complexType));
        }

        [Test]
        public void CreateFromFieldInfo()
        {
            var fieldInfo = typeof(ComplexType).GetField(nameof(ComplexType.value));
            var getterAndSetter = (GetterAndSetter<ComplexType, int>)GetterAndSetter.Create(fieldInfo);
            var complexType = new ComplexType();
            getterAndSetter.SetValue(complexType, 1);
            Assert.AreEqual(1, complexType.Value);
            Assert.AreEqual(1, getterAndSetter.GetValue(complexType));
        }
    }
}
