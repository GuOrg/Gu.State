namespace Gu.State
{
    using System;

    public class CustomCopy<T> : CustomCopy
    {
        private readonly Func<T, T, T> copyValue;

        public CustomCopy(Func<T, T, T> copyValue)
        {
            this.copyValue = copyValue;
        }

        public override object Copy(object source, object target)
        {
            return this.Copy((T)source, (T)target);
        }

        public T Copy(T source, T target)
        {
            return this.copyValue(source, target);
        }
    }
}