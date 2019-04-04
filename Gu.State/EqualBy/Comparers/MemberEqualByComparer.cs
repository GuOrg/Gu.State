namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("MemberEqualByComparer: {this.Member}")]
    internal class MemberEqualByComparer : EqualByComparer
    {
        private static readonly Lazy<EqualByComparer> EmptyLazy = new Lazy<EqualByComparer>(() => null);

        private readonly IGetterAndSetter getterAndSetter;
        private readonly Lazy<EqualByComparer> lazyComparer;

        private MemberEqualByComparer(MemberInfo member, Lazy<EqualByComparer> lazyComparer)
        {
            this.Member = member;
            if (!member.IsIndexer())
            {
                this.getterAndSetter = GetterAndSetter.GetOrCreate(member);
            }

            this.lazyComparer = lazyComparer;
        }

        internal MemberInfo Member { get; }

        internal override bool TryGetError(MemberSettings settings, out Error error)
        {
            if (this.lazyComparer.Value is ErrorEqualByComparer errorEqualByComparer)
            {
                error = errorEqualByComparer.Error;
                return true;
            }

            return settings.GetEqualByComparer(this.Member.MemberType()).TryGetError(settings, out error);
        }

        internal override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            if (this.getterAndSetter == null)
            {
                throw Throw.CompareWhenError;
            }

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
            if (member.IsIndexer())
            {
                return new MemberEqualByComparer(member, new Lazy<EqualByComparer>(() => new ErrorEqualByComparer(member.ReflectedType, UnsupportedIndexer.GetOrCreate((PropertyInfo)member))));
            }

            var memberType = member.MemberType();
            if (memberType.IsSealed)
            {
                return new MemberEqualByComparer(member, new Lazy<EqualByComparer>(() => settings.GetEqualByComparer(memberType)));
            }

            return new MemberEqualByComparer(member, EmptyLazy);
        }
    }
}