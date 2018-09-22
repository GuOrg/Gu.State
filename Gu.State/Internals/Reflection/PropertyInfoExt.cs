﻿namespace Gu.State
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class PropertyInfoExt
    {
        internal static bool IsCalculated(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.SetMethod != null)
            {
                return false;
            }

            var getMethod = propertyInfo.GetMethod;
            return !Attribute.IsDefined(getMethod, typeof(CompilerGeneratedAttribute));
        }

        internal static bool IsGetReadOnly(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.SetMethod != null)
            {
                return false;
            }

            var getMethod = propertyInfo.GetMethod;
            return Attribute.IsDefined(getMethod, typeof(CompilerGeneratedAttribute));
        }
    }
}
