namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    internal static partial class TypeExt
    {
        internal static bool IsEnumerableOfT(this Type type)
        {
            var iEnumerableOfT = type.GetIEnumerableOfT();
            return iEnumerableOfT != null;
        }

        internal static Type GetItemType(this Type type)
        {
            var enumerable = type.GetIEnumerableOfT();
            if (enumerable == null)
            {
                var message = $"Trying to get typeof(T) when type is not IEnumerable<T>, type is {type.Name}";
                throw new ArgumentException(message, nameof(type));
            }

            return enumerable.GetGenericArguments()
                             .Single();
        }

        private static Type GetIEnumerableOfT(this Type type)
        {
            var enumerable = type.GetInterfaces()
                                 .Where(i => i.IsGenericType)
                                 .SingleOrDefault(i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return enumerable;
        }

        public static bool IsNullable(this Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                // instantiated generic type only
                Type genericType = type.GetGenericTypeDefinition();
                return ReferenceEquals(genericType, typeof(Nullable<>));
            }

            return false;
        }

        internal static bool Implements<T>(this Type type)
        {
            return type.Implements(typeof(T));
        }

        /// <summary>
        /// To check if type implements IEquatable{string}
        /// Call like this type.Implements(typeof(IEquatable&lt;&gt;)
        /// </summary>
        internal static bool Implements(this Type type, Type @interface)
        {
            Debug.Assert(@interface.IsInterface, "genericInterface must be an interface type");
            var interfaces = type.GetInterfaces();
            if (interfaces.Contains(@interface))
            {
                return true;
            }

            if (!@interface.IsGenericTypeDefinition)
            {
                return false;
            }

            foreach (var i in interfaces)
            {
                if (!i.IsGenericType)
                {
                    continue;
                }

                var typeDefinition = i.GetGenericTypeDefinition();
                if (typeDefinition == @interface)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsOpenGenericType(this Type type)
        {
            return type.IsGenericTypeDefinition;
        }

        /// <summary>
        /// To check if type implements IEquatable{string}
        /// Call like this type.Implements(typeof(IEquatable&lt;&gt;, typeof(string))
        /// </summary>
        internal static bool Implements(this Type type, Type genericInterface, Type genericArgument)
        {
            if (type.IsInterface &&
                type.IsGenericType(genericInterface, genericArgument))
            {
                return true;
            }

            var interfaces = type.GetInterfaces();
            return interfaces.Any(i => i.IsGenericType(genericInterface, genericArgument));
        }

        internal static bool IsGenericType(this Type type, Type genericTypeDefinition, Type genericArgument)
        {
            Ensure.IsTrue(genericTypeDefinition.IsGenericType, nameof(genericTypeDefinition), $"{nameof(genericTypeDefinition)}.{nameof(genericTypeDefinition.IsGenericType)} must be true");

            if (!type.IsGenericType)
            {
                return false;
            }

            var gtd = type.GetGenericTypeDefinition();
            if (gtd != genericTypeDefinition)
            {
                return false;
            }

            var genericArguments = type.GetGenericArguments();
            return genericArguments.Length == 1 && genericArguments[0] == genericArgument;
        }
    }
}
