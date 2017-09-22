namespace Gu.State
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>Creates delegates for getting and setting the member.</summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    internal class GetterAndSetter<TSource, TValue> : IGetterAndSetter
    {
        private readonly Action<TSource, TValue> setter;
        private readonly Func<TSource, TValue> getter;

        public GetterAndSetter(PropertyInfo propertyInfo)
        {
            this.Member = propertyInfo;
            this.IsInitOnly = !propertyInfo.CanWrite;
            this.setter = propertyInfo.CanWrite
                              ? (Action<TSource, TValue>)propertyInfo.SetMethod.CreateDelegate(typeof(Action<TSource, TValue>))
                              : null;
            this.getter = propertyInfo.CanRead
                              ? (Func<TSource, TValue>)propertyInfo.GetMethod.CreateDelegate(typeof(Func<TSource, TValue>))
                              : null;
        }

        public GetterAndSetter(FieldInfo fieldInfo)
        {
            this.Member = fieldInfo;
            this.IsInitOnly = fieldInfo.IsInitOnly;
            this.setter = CreateSetterDelegate(fieldInfo);
            this.getter = CreateGetterDelegate(fieldInfo);
        }

        public Type SourceType => typeof(TSource);

        public Type ValueType => typeof(TValue);

        public bool IsInitOnly { get; }

        public MemberInfo Member { get; }

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

        public bool TryGetValueEquals(object x, object y, MemberSettings settings, out bool equal, out object xv, out object yv)
        {
            var result = this.TryGetValueEquals((TSource)x, (TSource)y, settings, out equal, out var xValue, out var yValue);
            xv = xValue;
            yv = yValue;
            return result;
        }

        public bool TryGetValueEquals(TSource x, TSource y, MemberSettings settings, out bool equal, out TValue xv, out TValue yv)
        {
            xv = this.GetValue(x);
            yv = this.GetValue(y);
            return EqualBy.TryGetValueEquals(xv, yv, settings, out equal);
        }

        public void CopyValue(object source, object target)
        {
            this.SetValue((TSource)target, this.GetValue((TSource)source));
        }

        public TValue GetValue(TSource source)
        {
            return this.getter(source);
        }

        // http://stackoverflow.com/a/16222886/1069200
        private static Action<TSource, TValue> CreateSetterDelegate(FieldInfo field)
        {
            //// ReSharper disable once PossibleNullReferenceException nope, not here
            var methodName = $"{field.ReflectedType.FullName}.set_{field.Name}";
            var setterMethod = new DynamicMethod(methodName, null, new[] { typeof(TSource), typeof(TValue) }, restrictedSkipVisibility: true);
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
            //// ReSharper disable once PossibleNullReferenceException nope, not here
            var methodName = $"{field.ReflectedType.FullName}.get_{field.Name}";
            var getterMethod = new DynamicMethod(methodName, typeof(TValue), new[] { typeof(TSource) }, restrictedSkipVisibility: true);
            var ilGenerator = getterMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, field);
            ilGenerator.Emit(OpCodes.Ret);
            return (Func<TSource, TValue>)getterMethod.CreateDelegate(typeof(Func<TSource, TValue>));
        }
    }
}