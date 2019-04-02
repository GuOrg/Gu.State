namespace Gu.State
{
    using System;
    using System.Linq;
    using System.Reflection;

    internal static class ComplexTypeEqualByComparer
    {
        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            comparer = (EqualByComparer)Activator.CreateInstance(
                typeof(Comparer<>).MakeGenericType(type),
                ImmutableArray.Create(settings.GetEffectiveMembers(type)
                                              .Select(m => MemberEqualByComparer.Create(m, settings))));
            return true;
        }

        private class Comparer<T> : EqualByComparer
        {
            private readonly ImmutableArray<MemberEqualByComparer> memberComparers;

            public Comparer(ImmutableArray<MemberEqualByComparer> memberComparers)
            {
                this.memberComparers = memberComparers;
            }

            public override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                return TryGetEitherNullEquals(x, y, out var result)
                    ? result
                    : this.Equals((T)x, (T)y, settings, referencePairs);
            }

            private bool Equals(T x, T y, MemberSettings settings, ReferencePairCollection referencePairs)
            {
                if (referencePairs != null &&
                    referencePairs.Add(x, y) == false)
                {
                    return true;
                }

                for (var i = 0; i < this.memberComparers.Count; i++)
                {
                    if (!this.memberComparers[i].Equals(x, y, settings, referencePairs))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private class MemberEqualByComparer : EqualByComparer
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

                    return settings.GetEqualByComparer(getterAndSetter.ValueType);
                }
            }
        }
    }
}