namespace Gu.State
{
    using System;

    public abstract class CustomCopy
    {
        public static CustomCopy Create<T>(Func<T, T, T> copyMethod)
        {
            return new CustomCopy<T>(copyMethod);
        }

        public abstract object Copy(object source, object target);
    }
}