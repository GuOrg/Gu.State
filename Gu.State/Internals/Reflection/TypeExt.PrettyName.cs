namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static partial class TypeExt
    {
        private static readonly IReadOnlyDictionary<Type, string> Aliases = new Dictionary<Type, string>
        {
            [typeof(string)] = "string",
            [typeof(bool)] = "bool",
            [typeof(bool?)] = "bool?",
            [typeof(byte)] = "byte",
            [typeof(byte?)] = "byte?",
            [typeof(char)] = "char",
            [typeof(char?)] = "char?",
            [typeof(decimal)] = "decimal",
            [typeof(decimal?)] = "decimal?",
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
        ///  Returns nicely formatted type names.
        ///  Useful for generic types.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The name formatted like : List{int}.</returns>
        internal static string PrettyName(this Type type)
        {
            if (type is null)
            {
                return "null";
            }

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                return $"{elementType.PrettyName()}[{new string(',', type.GetArrayRank() - 1)}]";
            }

            if (Aliases.TryGetValue(type, out var alias))
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
            if (Aliases.TryGetValue(type, out var alias))
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
