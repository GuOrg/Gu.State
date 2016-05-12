namespace Gu.State
{
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
                Member(source, target, settings, borrowed?.Value, member);
            }
        }

        private static void Sync<T>(T source, T target, IMemberSettings settings)
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
                Sync(source, target, settings, borrowed?.Value);
            }
        }

        private static void Sync<T>(T source, T target, IMemberSettings settings, ReferencePairCollection referencePairs)
        {
            Debug.Assert(source != null, nameof(source));
            Debug.Assert(target != null, nameof(target));
            Debug.Assert(source.GetType() == target.GetType(), "Must be same type");
            Verify.CanCopyMemberValues(source, target, settings);

            if (referencePairs?.Add(source, target) == false)
            {
                return;
            }

            T copy;
            if (TryCustomCopy(source, target, settings, out copy))
            {
                return;
            }

            CollectionItems(source, target, settings, referencePairs);
            Members(source, target, settings, referencePairs);
        }

        private static void Member<T>(
            T source,
            T target,
            IMemberSettings settings,
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
            IMemberSettings settings,
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

            var copy = Item(sv, tv, settings, referencePairs, false);
            if (!ReferenceEquals(tv, copy))
            {
                getterAndSetter.SetValue(target, copy);
            }
        }

        private static void Members<T>(T source, T target, IMemberSettings settings, ReferencePairCollection referencePairs)
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
    }
}