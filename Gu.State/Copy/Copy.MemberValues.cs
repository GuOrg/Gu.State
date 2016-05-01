namespace Gu.State
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

            using (var pairs = settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops
                                   ? ReferencePairCollection.Borrow()
                                   : null)
            {
                MemberValues.Copy(source, target, settings, member, pairs);
            }
        }

        private static class MemberValues
        {
            internal static void Copy<T>(
                T source,
                T target,
                IMemberSettings settings)
                where T : class
            {
                using (var pairs = settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops
                                       ? ReferencePairCollection.Borrow()
                                       : null)
                {
                    Copy(source, target, settings, pairs);
                }
            }

            internal static void Copy<T>(
              T source,
              T target,
              IMemberSettings settings,
              ReferencePairCollection referencePairs)
              where T : class
            {
                Debug.Assert(source != null, nameof(source));
                Debug.Assert(target != null, nameof(target));
                Debug.Assert(source.GetType() == target.GetType(), "Must be same type");
                Verify.CanCopyMemberValues(source, target, settings);
                if (referencePairs?.Contains(source, target) == true)
                {
                    return;
                }

                referencePairs?.Add(source, target);
                CopyCollectionItems(source, target, Copy, settings, referencePairs);
                MutableMembers(source, target, settings, referencePairs);
                InitiOnlyMembers(source, target, settings, referencePairs);
            }

            internal static void Copy<T>(
                T source,
                T target,
                IMemberSettings settings,
                MemberInfo member,
                ReferencePairCollection referencePairs)
                where T : class
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
                where T : class
            {
                Debug.Assert(source != null, nameof(source));
                Debug.Assert(target != null, nameof(target));
                Debug.Assert(source.GetType() == target.GetType(), "Must be same type");

                CopyCollectionItems(source, target, MutableMembers, settings, referencePairs);

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
                where T : class
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

                switch (settings.ReferenceHandling)
                {
                    case ReferenceHandling.References:
                        getterAndSetter.CopyValue(source, target);
                        return;
                    case ReferenceHandling.Structural:
                    case ReferenceHandling.StructuralWithReferenceLoops:

                        var tv = getterAndSetter.GetValue(target);
                        if (tv != null)
                        {
                            Copy(sv, tv, settings, referencePairs);
                            return;
                        }

                        tv = CreateInstance(sv, memberInfo, settings);
                        getterAndSetter.SetValue(target, tv);
                        Copy(sv, tv, settings, referencePairs);
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
                if (sv == null && tv == null)
                {
                    return;
                }

                if (!settings.IsImmutable(getterAndSetter.ValueType))
                {
                    Copy(sv, tv, settings, referencePairs);
                }

                if (!EqualBy.MemberValues(sv, tv, settings))
                {
                    Throw.ReadonlyMemberDiffers(new SourceAndTargetValue(source, sv, target, tv), memberInfo, settings);
                }
            }
        }
    }
}