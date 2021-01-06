namespace Gu.State
{
    using System;
    using System.Reflection;

    /// <summary>Creates delegates for getting and setting the member.</summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    internal class StructGetterAndSetter<TSource, TValue> : IGetterAndSetter
    {
        private readonly GetterDelegate getter;

        internal StructGetterAndSetter(PropertyInfo propertyInfo)
        {
            this.Member = propertyInfo;
            this.IsInitOnly = !propertyInfo.CanWrite;
            this.getter = propertyInfo.CanRead
                ? (GetterDelegate)Delegate.CreateDelegate(typeof(GetterDelegate), propertyInfo.GetMethod, throwOnBindFailure: true)
                : null;
        }

        private delegate TValue GetterDelegate(ref TSource source);

        public Type SourceType => typeof(TSource);

        public Type ValueType => typeof(TValue);

        public bool IsInitOnly { get; }

        public MemberInfo Member { get; }

        public void SetValue(object source, object value)
        {
            this.SetValue((TSource)source, (TValue)value);
        }

#pragma warning disable IDE0060, CA1801 // Review unused parameters
        public void SetValue(TSource source, TValue value) => throw new InvalidOperationException("Can't set value of struct.");
#pragma warning restore IDE0060, CA1801 // Review unused parameters

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
            return this.getter(ref source);
        }
    }
}
