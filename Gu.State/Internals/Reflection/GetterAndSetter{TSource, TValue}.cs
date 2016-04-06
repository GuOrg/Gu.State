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
            this.setter = (Action<TSource, TValue>)Delegate.CreateDelegate(typeof(Action<TSource, TValue>), propertyInfo.SetMethod);
            this.getter = (Func<TSource, TValue>)Delegate.CreateDelegate(typeof(Func<TSource, TValue>), propertyInfo.GetMethod);
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
            var gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, field);
            gen.Emit(OpCodes.Ret);
            return (Action<TSource, TValue>)setterMethod.CreateDelegate(typeof(Action<TSource, TValue>));
        }

        // http://stackoverflow.com/a/16222886/1069200
        private static Func<TSource, TValue> CreateGetterDelegate(FieldInfo field)
        {
            string methodName = $"{field.ReflectedType.FullName}.get_{field.Name}";
            DynamicMethod setterMethod = new DynamicMethod(methodName, typeof(TValue), new[] { typeof(TSource) }, true);
            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, field);
            gen.Emit(OpCodes.Ret);
            return (Func<TSource, TValue>)setterMethod.CreateDelegate(typeof(Func<TSource, TValue>));
        }
    }
}