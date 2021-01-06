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
            return PropertyCache.GetOrAdd(propertyInfo, x => Create(x));

            static IGetterAndSetter Create(PropertyInfo propertyInfo)
            {
                if (propertyInfo.DeclaringType.IsValueType)
                {
                    return Activator.CreateInstance<IGetterAndSetter>(
                        typeof(StructGetterAndSetter<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType),
                        new object[] { propertyInfo });
                }
                else
                {
                    return Activator.CreateInstance<IGetterAndSetter>(
                        typeof(GetterAndSetter<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType),
                        new object[] { propertyInfo });
                }
            }
        }

        internal static IGetterAndSetter GetOrCreate(FieldInfo fieldInfo)
        {
            return FieldCache.GetOrAdd(fieldInfo, x => Create(x));

            static IGetterAndSetter Create(FieldInfo fieldInfo)
            {
                return Activator.CreateInstance<IGetterAndSetter>(
                    typeof(GetterAndSetter<,>).MakeGenericType(fieldInfo.DeclaringType, fieldInfo.FieldType),
                    new object[] { fieldInfo });
            }
        }
    }
}
