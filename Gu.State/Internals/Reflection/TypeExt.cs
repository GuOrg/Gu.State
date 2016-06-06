namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    internal static partial class TypeExt
    {
        internal static bool IsKeyValuePair(this Type type)
        {
            return type.FullName.StartsWith("System.Collections.Generic.KeyValuePair`2");
        }

        internal static bool IsEnumerableOfT(this Type type)
        {
            var iEnumerableOfT = type.GetIEnumerableOfT();
            return iEnumerableOfT != null;
        }

        internal static bool IsDelegate(this Type type)
        {
            return typeof(Delegate).IsAssignableFrom(type);
        }

        internal static Type GetItemType(this Type type)
        {
            if (type.HasElementType)
            {
                return type.GetElementType();
            }

            if (type.Name == "IEnumerable`1")
            {
                return type.GetGenericArguments()
                           .Single();
            }

            var enumerable = type.GetIEnumerableOfT();
            if (enumerable == null)
            {
                var message = $"Trying to get typeof(T) when type is not IEnumerable<T>, type is {type.Name}";
                throw new ArgumentException(message, nameof(type));
            }

            return enumerable.GetGenericArguments()
                             .Single();
        }

        internal static bool IsNullable(this Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                // instantiated generic type only
                var genericType = type.GetGenericTypeDefinition();
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
        /// <param name="type">The type</param>
        /// <param name="interface">
        /// The interface type, can be an open interface IEnumerable{}
        /// </param>
        /// <returns>True if <paramref name="type"/> implements <paramref name="interface"/></returns>
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
        /// To check if type implements <paramref name="genericInterface"/>lt;<paramref name="genericArgument"/>&gt;
        /// Call like this type.Implements(typeof(IEquatable&lt;&gt;), typeof(string))
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="genericInterface">Example typeof(IEquatable&lt;&gt;)</param>
        /// <param name="genericArgument">Example typeof(string)</param>
        /// <returns>True is <paramref name="type"/> implements <paramref name="genericInterface"/>lt;<paramref name="genericArgument"/>&gt;</returns>
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

        private static Type GetIEnumerableOfT(this Type type)
        {
            var enumerable = type.GetInterfaces()
                                 .Where(i => i.IsGenericType)
                                 .SingleOrDefault(i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return enumerable;
        }
    }
}
