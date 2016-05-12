﻿namespace Gu.State
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    public static partial class Copy
    {
        internal static void Member<T>(
            T source,
            T target,
            IMemberSettings settings,
            MemberInfo member)
                where T : class
        {
            if (settings.IsIgnoringMember(member))
            {
                return;
            }

            using (var borrowed = settings.ReferenceHandling == ReferenceHandling.Structural
                                   ? ReferencePairCollection.Borrow()
                                   : null)
            {
                Member(source, target, settings, member, borrowed?.Value);
            }
        }

        private static void MemberValues<T>(T source, T target, IMemberSettings settings)
            where T : class
        {
            ////T copy;
            ////if (TryCustomCopy(source, target, settings, out copy))
            ////{
            ////    if (copy != null && !ReferenceEquals(target, copy))
            ////    {
            ////        var message = $"The type {source.GetType()} has custom copy specified. For the root object the copy must be a side effect.\r\n" +
            ////                      $"This means that the custom copy must return null or the target instance.\r\n" +
            ////                      $"Also it makes little sense using this method with custom copy for the root type.";
            ////        throw new InvalidOperationException(message);
            ////    }

            ////    return copy;
            ////}

            using (var borrowed = settings.ReferenceHandling == ReferenceHandling.Structural
                                   ? ReferencePairCollection.Borrow()
                                   : null)
            {
                MemberValues(source, target, settings, borrowed?.Value);
                return;
            }
        }

        private static T MemberValues<T>(
          T source,
          T target,
          IMemberSettings settings,
          ReferencePairCollection referencePairs)
        {
            Debug.Assert(source != null, nameof(source));
            Debug.Assert(target != null, nameof(target));
            Debug.Assert(source.GetType() == target.GetType(), "Must be same type");
            Verify.CanCopyMemberValues(source, target, settings);

            if (referencePairs?.Add(source, target) == false)
            {
                return target;
            }

            T copy;
            if (TryCustomCopy(source, target, settings, out copy))
            {
                return copy;
            }

            CollectionItems(source, target, MemberValues, settings, referencePairs);
            MutableMembers(source, target, settings, referencePairs);
            InitiOnlyMembers(source, target, settings, referencePairs);
            return target;
        }

        private static void Member<T>(
            T source,
            T target,
            IMemberSettings settings,
            MemberInfo member,
            ReferencePairCollection referencePairs)
        {
            var getterAndSetter = settings.GetOrCreateGetterAndSetter(member);
            if (getterAndSetter.IsInitOnly)
            {
                InitOnlyMember(source, target, settings, referencePairs, member);
            }
            else
            {
                MutableMember(source, target, settings, referencePairs, member);
            }
        }

        private static void MutableMembers<T>(T source, T target, IMemberSettings settings, ReferencePairCollection referencePairs)
        {
            Debug.Assert(source != null, nameof(source));
            Debug.Assert(target != null, nameof(target));
            Debug.Assert(source.GetType() == target.GetType(), "Must be same type");

            foreach (var memberInfo in settings.GetMembers(source.GetType()))
            {
                MutableMember(source, target, settings, referencePairs, memberInfo);
            }
        }

        private static void MutableMember<T>(
            T source,
            T target,
            IMemberSettings settings,
            ReferencePairCollection referencePairs,
            MemberInfo memberInfo)
        {
            if (settings.IsIgnoringMember(memberInfo))
            {
                return;
            }

            var getterAndSetter = settings.GetOrCreateGetterAndSetter(memberInfo);
            if (getterAndSetter.IsInitOnly)
            {
                return;
            }

            var sv = getterAndSetter.GetValue(source);
            if (settings.IsImmutable(getterAndSetter.ValueType) || sv == null)
            {
                getterAndSetter.CopyValue(source, target);
                return;
            }

            object copy;
            if (TryCustomCopy(sv, getterAndSetter.GetValue(target), settings, out copy))
            {
                getterAndSetter.SetValue(target, copy);
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.References:
                    getterAndSetter.CopyValue(source, target);
                    return;
                case ReferenceHandling.Structural:
                    var tv = getterAndSetter.GetValue(target);
                    if (tv != null)
                    {
                        MemberValues(sv, tv, settings, referencePairs);
                        return;
                    }

                    tv = CreateInstance(sv, memberInfo, settings);
                    getterAndSetter.SetValue(target, tv);
                    MemberValues(sv, tv, settings, referencePairs);
                    return;
                case ReferenceHandling.Throw:
                    throw State.Throw.ShouldNeverGetHereException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
            }
        }

        private static void InitiOnlyMembers(
            object source,
            object target,
            IMemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            foreach (var memberInfo in settings.GetMembers(source.GetType()))
            {
                InitOnlyMember(source, target, settings, referencePairs, memberInfo);
            }
        }

        private static void InitOnlyMember(
            object source,
            object target,
            IMemberSettings settings,
            ReferencePairCollection referencePairs,
            MemberInfo memberInfo)
        {
            if (settings.IsIgnoringMember(memberInfo))
            {
                return;
            }

            var getterAndSetter = settings.GetOrCreateGetterAndSetter(memberInfo);
            if (!getterAndSetter.IsInitOnly)
            {
                return;
            }

            var sv = getterAndSetter.GetValue(source);
            var tv = getterAndSetter.GetValue(target);
            bool equals;
            if (EqualBy.TryGetValueEquals(source, target, settings, out equals) && equals)
            {
                return;
            }

            object copy;
            if (TryCustomCopy(sv, tv, settings, out copy))
            {
                // nop called for side effect. Checked for equality below.
            }
            else if (!settings.IsImmutable(getterAndSetter.ValueType))
            {
                MemberValues(sv, tv, settings, referencePairs);
            }

            if (!EqualBy.MemberValues(sv, tv, settings))
            {
                Throw.ReadonlyMemberDiffers(new SourceAndTargetValue(source, sv, target, tv), memberInfo, settings);
            }
        }
    }
}