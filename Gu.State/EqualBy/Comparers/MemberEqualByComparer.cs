namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;

    [DebuggerDisplay("MemberEqualByComparer: {this.Member}")]
    internal abstract class MemberEqualByComparer : EqualByComparer
    {
        protected MemberEqualByComparer(MemberInfo member)
        {
            this.Member = member;
        }

        internal MemberInfo Member { get; }

        internal static MemberEqualByComparer Create(MemberInfo member, MemberSettings settings)
        {
            if (member.IsIndexer())
            {
                return new Indexer(member);
            }

            var memberType = member.MemberType();
            if (memberType.IsSealed)
            {
                var equalByComparer = settings.GetEqualByComparer(memberType);
                if (equalByComparer.GetType().IsGenericType &&
                    equalByComparer.GetType().GetGenericTypeDefinition() == typeof(ExplicitEqualByComparer<>))
                {
                    switch (settings)
                    {
                        case PropertiesSettings _:
                            return (MemberEqualByComparer)typeof(SealedMemberEqualByComparer)
                                                           .GetMethod(nameof(SealedMemberEqualByComparer.CreateForProperty), BindingFlags.NonPublic | BindingFlags.Static)
                                                           .MakeGenericMethod(member.ReflectedType, memberType)
                                                           .Invoke(null, new object[] { member, equalByComparer });
                        case FieldsSettings _:
                            return (MemberEqualByComparer)typeof(SealedMemberEqualByComparer)
                                                          .GetMethod(nameof(SealedMemberEqualByComparer.CreateForField), BindingFlags.NonPublic | BindingFlags.Static)
                                                          .MakeGenericMethod(member.ReflectedType, memberType)
                                                          .Invoke(null, new object[] { member, equalByComparer });
                    }
                }

                return new Comparer(member, equalByComparer);
            }

            return new Comparer(member, (EqualByComparer)Activator.CreateInstance(typeof(LazyEqualByComparer<>).MakeGenericType(memberType)));
        }

        private class Comparer : MemberEqualByComparer
        {
            private readonly IGetterAndSetter getterAndSetter;
            private readonly EqualByComparer comparer;

            internal Comparer(MemberInfo member, EqualByComparer comparer)
                : base(member)
            {
                this.getterAndSetter = GetterAndSetter.GetOrCreate(member);
                this.comparer = comparer;
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

        private class SealedMemberEqualByComparer : MemberEqualByComparer
        {
            private readonly Func<object, object, bool> equals;

            private SealedMemberEqualByComparer(MemberInfo member, Func<object, object, bool> equals)
                : base(member)
            {
                this.equals = equals;
            }

            internal static SealedMemberEqualByComparer CreateForProperty<TSource, TValue>(PropertyInfo property, ExplicitEqualByComparer<TValue> comparer)
            {
                var getter = CreateGetter();
                return new SealedMemberEqualByComparer(property, (x, y) => comparer.EqualityComparer.Equals(getter((TSource)x), getter((TSource)y)));
                Func<TSource, TValue> CreateGetter()
                {
                    var parameter = Expression.Parameter(typeof(TSource));
                    return Expression.Lambda<Func<TSource, TValue>>(Expression.Property(parameter, property), parameter)
                                     .Compile();
                }
            }

            internal static SealedMemberEqualByComparer CreateForField<TSource, TValue>(FieldInfo field, ExplicitEqualByComparer<TValue> comparer)
            {
                var getter = CreateGetter();
                return new SealedMemberEqualByComparer(field, (x, y) => comparer.EqualityComparer.Equals(getter((TSource)x), getter((TSource)y)));
                Func<TSource, TValue> CreateGetter()
                {
                    var parameter = Expression.Parameter(typeof(TSource));
                    return Expression.Lambda<Func<TSource, TValue>>(Expression.Field(parameter, field), parameter)
                                     .Compile();
                }
            }

            internal override bool TryGetError(MemberSettings settings, out Error error)
            {
                error = null;
                return false;
            }

            internal override bool Equals(object x, object y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs) => this.equals(x, y);
        }

        private class Indexer : MemberEqualByComparer
        {
            private readonly UnsupportedIndexer error;

            public Indexer(MemberInfo member)
                : base(member)
            {
                this.error = UnsupportedIndexer.GetOrCreate((PropertyInfo)member);
            }

            internal override bool TryGetError(MemberSettings settings, out Error error)
            {
                error = this.error;
                return true;
            }

            internal override bool Equals(object x, object y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
            {
                throw Throw.CompareWhenError;
            }
        }
    }
}