namespace Gu.State
{
    using System.Collections.Concurrent;
    using System.Reflection;

    internal class GetterAndSetter
    {
        private static readonly ConcurrentDictionary<PropertyInfo, IGetterAndSetter> PropertyCache = new ConcurrentDictionary<PropertyInfo, IGetterAndSetter>();

        internal static IGetterAndSetter GetOrCreate(PropertyInfo propertyInfo)
        {
            return PropertyCache.GetOrAdd(propertyInfo, Create);
        }

        internal static IGetterAndSetter GetOrCreate(FieldInfo fieldInfo)
        {
            var setter = typeof(GetterAndSetter<,>).MakeGenericType(fieldInfo.DeclaringType, fieldInfo.FieldType);
            var constructorInfo = setter.GetConstructor(new[] { typeof(FieldInfo) });
            return (IGetterAndSetter)constructorInfo.Invoke(new object[] { fieldInfo });
        }

        private static IGetterAndSetter Create(PropertyInfo propertyInfo)
        {
            var setter = typeof(GetterAndSetter<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType);
            var constructorInfo = setter.GetConstructor(new[] { typeof(PropertyInfo) });
            return (IGetterAndSetter)constructorInfo.Invoke(new object[] { propertyInfo });
        }
    }
}