namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("MemberEqualByComparer: {this.Member}")]
    internal class MemberEqualByComparer : EqualByComparer
    {
        private readonly IGetterAndSetter getterAndSetter;
        private readonly EqualByComparer comparer;

        private MemberEqualByComparer(MemberInfo member, EqualByComparer comparer)
        {
            this.Member = member;
            if (!member.IsIndexer())
            {
                this.getterAndSetter = GetterAndSetter.GetOrCreate(member);
            }

            this.comparer = comparer;
        }

        internal MemberInfo Member { get; }

        internal static MemberEqualByComparer Create(MemberInfo member, MemberSettings settings)
        {
            if (member.IsIndexer())
            {
                return new MemberEqualByComparer(member, new ErrorEqualByComparer(member.ReflectedType, UnsupportedIndexer.GetOrCreate((PropertyInfo)member)));
            }

            var memberType = member.MemberType();
            if (memberType.IsSealed)
            {
                return new MemberEqualByComparer(member, settings.GetEqualByComparer(memberType));
            }

            return new MemberEqualByComparer(member, (EqualByComparer)Activator.CreateInstance(typeof(LazyEqualByComparer<>).MakeGenericType(memberType)));
        }

        internal override bool TryGetError(MemberSettings settings, out Error error)
        {
            if (this.comparer is ErrorEqualByComparer errorEqualByComparer)
            {
                error = errorEqualByComparer.Error;
                return true;
            }

            return settings.GetEqualByComparer(this.Member.MemberType()).TryGetError(settings, out error);
        }

        internal override bool Equals(object x, object y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
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

            return this.comparer.Equals(xv, yv, settings, referencePairs);
        }
    }
}