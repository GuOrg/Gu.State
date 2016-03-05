namespace Gu.ChangeTracking
{
    using System;

    internal abstract class TypeError : Error
    {
        protected TypeError(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; }
    }
}