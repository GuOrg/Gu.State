namespace Gu.ChangeTracking
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    internal static partial class TypeExt
    {
        private static readonly ConcurrentDictionary<Type, string> Aliases = new ConcurrentDictionary<Type, string>
        {
            [typeof(bool)] = "bool",
            [typeof(bool?)] = "bool?",
            [typeof(byte)] = "byte",
            [typeof(byte?)] = "byte?",
            [typeof(char)] = "char",
            [typeof(char?)] = "char?",
            [typeof(double)] = "double",
            [typeof(double?)] = "double?",
            [typeof(float)] = "float",
            [typeof(float?)] = "float?",
            [typeof(short)] = "short",
            [typeof(short?)] = "short?",
            [typeof(int)] = "int",
            [typeof(int?)] = "int?",
            [typeof(long)] = "long",
            [typeof(long?)] = "long?",
            [typeof(sbyte)] = "sbyte",
            [typeof(sbyte?)] = "sbyte?",
            [typeof(ushort)] = "ushort",
            [typeof(ushort?)] = "ushort?",
            [typeof(uint)] = "uint",
            [typeof(uint?)] = "uint?",
            [typeof(ulong)] = "ulong",
            [typeof(ulong?)] = "ulong?",
        };

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

        /// <summary>
        /// Returns nicely formatted type names for generic types.
        /// </summary>
        internal static string PrettyName(this Type type)
        {
            string alias;
            if (Aliases.TryGetValue(type, out alias))
            {
                return alias;
            }

            if (type.IsGenericType)
            {
                var arguments = string.Join(", ", type.GenericTypeArguments.Select(PrettyName));
                return $"{type.Name.Split('`').First()}<{arguments}>";
            }

            return type.Name;
        }

        internal static string FullPrettyName(this Type type)
        {
            string alias;
            if (Aliases.TryGetValue(type, out alias))
            {
                return alias;
            }

            if (type.IsGenericType)
            {
                var arguments = string.Join(", ", type.GenericTypeArguments.Select(FullPrettyName));
                var fullPrettyName = $"{type.FullName.Replace("+", ".").Split('`').First()}<{arguments}>";
                return fullPrettyName;
            }

            return type.FullName.Replace("+", ".");
        }

        public static bool IsEquatable(this Type type)
        {
            return type.Implements(typeof(IEquatable<>), type);
        }

        public static bool IsNullable(this Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                // instantiated generic type only
                Type genericType = type.GetGenericTypeDefinition();
                return object.ReferenceEquals(genericType, typeof(Nullable<>));
            }

            return false;
        }

        /// <summary>
        /// To check if type implements IEquatable{string}
        /// Call like this type.Implements(typeof(IEquatable{}, typeof(string))
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
