namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    internal class GetterAndSetter<TSource, TValue> : IGetterAndSetter
    {
        public readonly Action<TSource, TValue> Setter;
        public readonly Func<TSource, TValue> Getter;

        public GetterAndSetter(PropertyInfo propertyInfo)
        {
            this.Setter = propertyInfo.CanWrite
                              ? (Action<TSource, TValue>)
                                propertyInfo.SetMethod.CreateDelegate(typeof(Action<TSource, TValue>))
                              : null;
            this.Getter = propertyInfo.CanRead
                              ? (Func<TSource, TValue>)
                                propertyInfo.GetMethod.CreateDelegate(typeof(Func<TSource, TValue>))
                              : null;
        }

        public Type SourceType => typeof(TSource);

        public Type ValueType => typeof(TValue);

        public GetterAndSetter(FieldInfo fieldInfo)
        {
            this.Setter = CreateSetterDelegate(fieldInfo);
            this.Getter = CreateGetterDelegate(fieldInfo);
        }

        public void SetValue(object source, object value)
        {
            this.SetValue((TSource)source, (TValue)value);
        }

        public void SetValue(TSource source, TValue value)
        {
            this.Setter(source, value);
        }

        public object GetValue(object source)
        {
            return this.GetValue((TSource)source);
        }

        public bool ValueEquals(object x, object y)
        {
            var xv = this.GetValue((TSource)x);
            var yv = this.GetValue((TSource)y);
            var comparer = EqualityComparer<TValue>.Default;
            return comparer.Equals(xv, yv);
        }

        public TValue GetValue(TSource source)
        {
            return this.Getter(source);
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
            var getterMethod = new DynamicMethod(methodName, typeof(TValue), new[] { typeof(TSource) }, true);
            var ilGenerator = getterMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, field);
            ilGenerator.Emit(OpCodes.Ret);
            return (Func<TSource, TValue>)getterMethod.CreateDelegate(typeof(Func<TSource, TValue>));
        }
    }
}