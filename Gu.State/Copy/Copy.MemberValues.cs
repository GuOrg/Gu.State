namespace Gu.State
{
    using System.Diagnostics;
    using System.Reflection;

    public static partial class Copy
    {
        internal static void Member<T>(
            T source,
            T target,
            MemberSettings settings,
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
                Member(source, target, settings, borrowed?.Value, member);
            }
        }

        private static void Members<T>(T source, T target, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            Debug.Assert(source != null, nameof(source));
            Debug.Assert(target != null, nameof(target));
            Debug.Assert(source.GetType() == target.GetType(), "Must be same type");

            using (var borrowed = ListPool<IGetterAndSetter>.Borrow())
            {
                foreach (var member in settings.GetMembers(source.GetType()))
                {
                    if (settings.IsIgnoringMember(member))
                    {
                        continue;
                    }

                    var getterAndSetter = settings.GetOrCreateGetterAndSetter(member);
                    Member(source, target, settings, referencePairs, getterAndSetter);
                    if (getterAndSetter.IsInitOnly)
                    {
                        borrowed.Value.Add(getterAndSetter);
                    }
                }

                foreach (var getterAndSetter in borrowed.Value)
                {
                    var sv = getterAndSetter.GetValue(source);
                    var tv = getterAndSetter.GetValue(target);

                    if (!EqualBy.MemberValues(sv, tv, settings))
                    {
                        Throw.ReadonlyMemberDiffers(new SourceAndTargetValue(source, sv, target, tv), getterAndSetter.Member, settings);
                    }
                }
            }
        }

        private static void Member<T>(
            T source,
            T target,
            MemberSettings settings,
            ReferencePairCollection referencePairs,
            MemberInfo member)
        {
            if (settings.IsIgnoringMember(member))
            {
                return;
            }

            var getterAndSetter = settings.GetOrCreateGetterAndSetter(member);
            Member(source, target, settings, referencePairs, getterAndSetter);
        }

        private static void Member<T>(
            T source,
            T target,
            MemberSettings settings,
            ReferencePairCollection referencePairs,
            IGetterAndSetter getterAndSetter)
        {
            var sv = getterAndSetter.GetValue(source);
            var tv = getterAndSetter.GetValue(target);
            if (getterAndSetter.IsInitOnly)
            {
                if (settings.IsImmutable(getterAndSetter.ValueType) || sv == null)
                {
                    return;
                }

                Sync(sv, tv, settings, referencePairs);
                return;
            }

            if (settings.IsImmutable(getterAndSetter.ValueType) || sv == null)
            {
                getterAndSetter.CopyValue(source, target);
                return;
            }

            bool created;
            bool needsSync;
            var clone = CloneWithoutSync(sv, tv, settings, out created, out needsSync);
            if (created)
            {
                getterAndSetter.SetValue(target, clone);
            }

            if (needsSync)
            {
                Sync(sv, clone, settings, referencePairs);
            }
        }
    }
}