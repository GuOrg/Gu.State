namespace Gu.State
{
    using System;
    using System.Reflection;

    internal class MemberEqualByComparer : EqualByComparer
    {
        private readonly IGetterAndSetter getterAndSetter;
        private readonly Lazy<EqualByComparer> comparer;

        private MemberEqualByComparer(IGetterAndSetter getterAndSetter, Lazy<EqualByComparer> comparer)
        {
            this.getterAndSetter = getterAndSetter;
            this.comparer = comparer;
        }

        public override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            var xv = this.getterAndSetter.GetValue(x);
            var yv = this.getterAndSetter.GetValue(y);
            if (TryGetEitherNullEquals(xv, yv, out var result))
            {
                return result;
            }

            return this.comparer.Value.Equals(xv, yv, settings, referencePairs);
        }

        internal static MemberEqualByComparer Create(MemberInfo member, MemberSettings settings)
        {
            var getterAndSetter = settings.GetOrCreateGetterAndSetter(member);

            return new MemberEqualByComparer(getterAndSetter, new Lazy<EqualByComparer>(() => settings.GetEqualByComparer(getterAndSetter.ValueType)));
        }
    }
}