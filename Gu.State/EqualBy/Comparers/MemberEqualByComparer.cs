namespace Gu.State
{
    using System;
    using System.Reflection;

    internal partial class MemberEqualByComparer : EqualByComparer
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

            return new MemberEqualByComparer(getterAndSetter, new Lazy<EqualByComparer>(() => GetComparer()));

            EqualByComparer GetComparer()
            {
                var memberType = member.MemberType();
                if (!memberType.IsValueType &&
                    !settings.IsEquatable(memberType) &&
                    !settings.TryGetComparer(memberType, out _))
                {
                    switch (settings.ReferenceHandling)
                    {
                        case ReferenceHandling.Throw:
                            throw new InvalidOperationException($"Member {member} is a reference type {member.MemberType().PrettyName()} and ReferenceHandling.Throw is used.");
                        case ReferenceHandling.References:
                            return ReferenceEqualByComparer.Default;
                        case ReferenceHandling.Structural:
                            break;
                    }
                }

                if (memberType.IsSealed)
                {
                    return settings.GetEqualByComparer(getterAndSetter.ValueType);
                }

                return LazyTypeEqualByComparer.Default;
            }
        }
    }
}