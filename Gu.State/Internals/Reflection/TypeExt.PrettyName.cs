namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static partial class TypeExt
    {
        private static readonly IReadOnlyDictionary<Type, string> Aliases = new Dictionary<Type, string>
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

        /// <summary>
        /// Returns nicely formatted type names for generic types.
        /// </summary>
        internal static string PrettyName(this Type type)
        {
            if (type == null)
            {
                return "null";
            }

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                return elementType.PrettyName() + "[]";
            }

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
    }
}
