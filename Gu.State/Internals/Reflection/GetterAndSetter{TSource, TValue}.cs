namespace Gu.State
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    internal class GetterAndSetter<TSource, TValue> : IGetterAndSetter
    {
        private readonly Action<TSource, TValue> setter;
        private readonly Func<TSource, TValue> getter;

        public GetterAndSetter(PropertyInfo propertyInfo)
        {
            this.setter = (Action<TSource, TValue>)propertyInfo.SetMethod.CreateDelegate(typeof(Action<TSource, TValue>));
            this.getter = (Func<TSource, TValue>)propertyInfo.GetMethod.CreateDelegate(typeof(Func<TSource, TValue>));
        }

        public GetterAndSetter(FieldInfo fieldInfo)
        {
            this.setter = CreateSetterDelegate(fieldInfo);
            this.getter = CreateGetterDelegate(fieldInfo);
        }

        public void SetValue(object source, object value)
        {
            this.SetValue((TSource)source, (TValue)value);
        }

        public void SetValue(TSource source, TValue value)
        {
            this.setter(source, value);
        }

        public object GetValue(object source)
        {
            return this.GetValue((TSource)source);
        }

        public TValue GetValue(TSource source)
        {
            return this.getter(source);
        }

        // http://stackoverflow.com/a/16222886/1069200
        private static Action<TSource, TValue> CreateSetterDelegate(FieldInfo field)
        {
            var methodName = $"{field.ReflectedType.FullName}.set_{field.Name}";
            var setterMethod = new DynamicMethod(methodName, null, new[] { typeof(TSource), typeof(TValue) }, true);
            var ilGenerator = setterMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stfld, field);
            ilGenerator.Emit(OpCodes.Ret);
            return (Action<TSource, TValue>)setterMethod.CreateDelegate(typeof(Action<TSource, TValue>));
        }

        // http://stackoverflow.com/a/16222886/1069200
        private static Func<TSource, TValue> CreateGetterDelegate(FieldInfo field)
        {
            var methodName = $"{field.ReflectedType.FullName}.get_{field.Name}";
            var setterMethod = new DynamicMethod(methodName, typeof(TValue), new[] { typeof(TSource) }, true);
            var ilGenerator = setterMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, field);
            ilGenerator.Emit(OpCodes.Ret);
            return (Func<TSource, TValue>)setterMethod.CreateDelegate(typeof(Func<TSource, TValue>));
        }
    }
}