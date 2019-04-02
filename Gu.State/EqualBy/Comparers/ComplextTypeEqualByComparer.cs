namespace Gu.State
{
    using System;
    using System.Linq;

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

        private class Comparer<T> : EqualByComparer<T>
        {
            private readonly ImmutableArray<MemberEqualByComparer> memberComparers;

            public Comparer(ImmutableArray<MemberEqualByComparer> memberComparers)
            {
                this.memberComparers = memberComparers;
            }

            public override bool Equals(T x, T y, MemberSettings settings, ReferencePairCollection referencePairs)
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
    }
}