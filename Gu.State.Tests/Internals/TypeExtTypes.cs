namespace Gu.State.Tests.Internals
{
    using System;
    using System.Collections.Generic;

    public static class TypeExtTypes
    {
        interface IMutableInterface
        {
            int MutableValue { get; set; }
        }

        public class WithGetPrivateSet
        {
            public int Value { get; private set; }
        }

        public class WithGetPublicSet
        {
            public int Value { get; set; }
        }

        public class WithGetReadOnlyProperty<T>
        {
            public T Value { get; }
        }

        public sealed class WithGetReadOnlyPropertySealed<T> : IEquatable<WithGetReadOnlyPropertySealed<T>>
        {
            public T Value { get; }

            public bool Equals(WithGetReadOnlyPropertySealed<T> other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return EqualityComparer<T>.Default.Equals(this.Value, other.Value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                return obj is WithGetReadOnlyPropertySealed<T> && this.Equals((WithGetReadOnlyPropertySealed<T>)obj);
            }

            public override int GetHashCode()
            {
                return EqualityComparer<T>.Default.GetHashCode(this.Value);
            }
        }

        public struct WithGetReadOnlyPropertyStruct<T>
        {
            public T Value { get; }
        }

        public class WithImmutableImplementingMutableInterfaceExplicit : IMutableInterface
        {
            public readonly int ImmutableValue;
            int IMutableInterface.MutableValue { get; set; }
        }

        public class WithImmutableSubclassingMutable : WithMutableField
        {
            public readonly int ImmutableValue;
        }

        public class WithMutableField
        {
            public int Value;
        }

        public class WithReadonlyField<T>
        {
            public readonly T Value;
        }

        public sealed class WithReadonlyFieldSealed<T>
        {
            public readonly T Value;
        }

        public class WithSelfField
        {
            public readonly WithSelfField Value;
        }

        public sealed class WithSelfFieldSealed
        {
            public readonly WithSelfFieldSealed Value;
        }

        public class WithSelfProp
        {
            public WithSelfProp Value { get; }
        }

        public sealed class WithSelfPropSealed
        {
            public WithSelfPropSealed Value { get; }
        }
    }
}
