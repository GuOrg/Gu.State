namespace Gu.State
{
    using System.Reflection;

    internal static class GetterAndSetter
    {
        internal static IGetterAndSetter Create(PropertyInfo propertyInfo)
        {
            var setter = typeof(GetterAndSetter<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType);
            var constructorInfo = setter.GetConstructor(new[] { typeof(PropertyInfo) });
            return (IGetterAndSetter)constructorInfo.Invoke(new object[] { propertyInfo });
        }

        internal static IGetterAndSetter Create(FieldInfo fieldInfo)
        {
            var setter = typeof(GetterAndSetter<,>).MakeGenericType(fieldInfo.DeclaringType, fieldInfo.FieldType);
            var constructorInfo = setter.GetConstructor(new[] { typeof(FieldInfo) });
            return (IGetterAndSetter)constructorInfo.Invoke(new object[] { fieldInfo });
        }
    }
}