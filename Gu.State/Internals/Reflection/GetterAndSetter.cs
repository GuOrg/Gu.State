namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    internal static class GetterAndSetter
    {
        private static readonly ConcurrentDictionary<PropertyInfo, IGetterAndSetter> PropertyCache = new ConcurrentDictionary<PropertyInfo, IGetterAndSetter>();
        private static readonly ConcurrentDictionary<FieldInfo, IGetterAndSetter> FieldCache = new ConcurrentDictionary<FieldInfo, IGetterAndSetter>();

        internal static IGetterAndSetter GetOrCreate(MemberInfo member)
        {
            return member switch
            {
                PropertyInfo property => GetOrCreate(property),
                FieldInfo field => GetOrCreate(field),
                _ => throw new ArgumentOutOfRangeException(nameof(member), member, $"Cannot create IGetterAndSetter for member {member}."),
            };
        }

        internal static IGetterAndSetter GetOrCreate(PropertyInfo propertyInfo)
        {
            return PropertyCache.GetOrAdd(propertyInfo, Create);
        }

        internal static IGetterAndSetter GetOrCreate(FieldInfo fieldInfo)
        {
            return FieldCache.GetOrAdd(fieldInfo, Create);
        }

        private static IGetterAndSetter Create(FieldInfo fieldInfo)
        {
            var setter = typeof(GetterAndSetter<,>).MakeGenericType(fieldInfo.DeclaringType, fieldInfo.FieldType);
            var constructorInfo = setter.GetConstructor(new[] { typeof(FieldInfo) });
            //// ReSharper disable once PossibleNullReferenceException nope, not here
            return (IGetterAndSetter)constructorInfo.Invoke(new object[] { fieldInfo });
        }

        private static IGetterAndSetter Create(PropertyInfo propertyInfo)
        {
            if (propertyInfo.DeclaringType.IsValueType)
            {
                return (IGetterAndSetter)Activator.CreateInstance(
                    typeof(StructGetterAndSetter<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType),
                    propertyInfo);
            }
            else
            {
                return (IGetterAndSetter)Activator.CreateInstance(
                    typeof(GetterAndSetter<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType),
                    propertyInfo);
            }
        }
    }
}
