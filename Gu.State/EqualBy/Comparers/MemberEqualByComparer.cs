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
            return TryGetEitherNullEquals(x, y, out var result)
                ? result
                : this.comparer.Value.Equals(this.getterAndSetter.GetValue(x), this.getterAndSetter.GetValue(y), settings, referencePairs);
        }

        internal static MemberEqualByComparer Create(MemberInfo member, MemberSettings settings)
        {
            var getterAndSetter = settings.GetOrCreateGetterAndSetter(member);

            return new MemberEqualByComparer(getterAndSetter, new Lazy<EqualByComparer>(() => settings.GetEqualByComparer(getterAndSetter.ValueType, checkReferenceHandling: true)));
        }
    }
}