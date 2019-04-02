namespace Gu.State
{
    using System;
    using System.Reflection;

    internal class MemberEqualByComparer : EqualByComparer
    {
        private static readonly Lazy<EqualByComparer> EmptyLazy = new Lazy<EqualByComparer>(() => null);

        private readonly IGetterAndSetter getterAndSetter;
        private readonly Lazy<EqualByComparer> lazyComparer;

        private MemberEqualByComparer(IGetterAndSetter getterAndSetter, Lazy<EqualByComparer> lazyComparer)
        {
            this.getterAndSetter = getterAndSetter;
            this.lazyComparer = lazyComparer;
        }

        public override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            var xv = this.getterAndSetter.GetValue(x);
            var yv = this.getterAndSetter.GetValue(y);
            if (TryGetEitherNullEquals(xv, yv, out var result))
            {
                return result;
            }

            var comparer = this.lazyComparer.Value ?? settings.GetEqualByComparer(xv.GetType());
            return comparer.Equals(xv, yv, settings, referencePairs);
        }

        internal static MemberEqualByComparer Create(MemberInfo member, MemberSettings settings)
        {
            var getterAndSetter = settings.GetOrCreateGetterAndSetter(member);

            if (getterAndSetter.ValueType.IsSealed)
            {
                return new MemberEqualByComparer(getterAndSetter, new Lazy<EqualByComparer>(() => settings.GetEqualByComparer(getterAndSetter.ValueType)));
            }

            return new MemberEqualByComparer(getterAndSetter, EmptyLazy);
        }
    }
}