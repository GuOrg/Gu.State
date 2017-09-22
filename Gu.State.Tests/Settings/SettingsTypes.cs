// ReSharper disable All
#pragma warning disable 649
#pragma warning disable WPF1011 // Implement INotifyPropertyChanged.
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
#pragma warning disable SA1401 // Fields must be private
namespace Gu.State.Tests.Settings
{
    using System;
    using System.Collections.Generic;

    public static class SettingsTypes
    {
        private interface IMutableInterface
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

        public interface IComplexType
        {
            string Name { get; set; }

            int Value { get; set; }
        }

        public class ComplexType : IComplexType
        {
            internal string name;
            internal int value;

            public string Name
            {
                get { return this.name; }
                set { this.name = value; }
            }

            public int Value
            {
                get { return this.value; }
                set { this.value = value; }
            }
        }

        public class Derived : ComplexType
        {
            internal double doubleValue;

            public double DoubleValue
            {
                get
                {
                    return this.doubleValue;
                }

                set
                {
                    this.doubleValue = value;
                }
            }
        }

        public sealed class Immutable : IEquatable<Immutable>
        {
            internal readonly int value;

            public int Value => this.value;

            public static bool operator ==(Immutable left, Immutable right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Immutable left, Immutable right)
            {
                return !Equals(left, right);
            }

            public bool Equals(Immutable other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return this.Value == other.Value;
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

                return obj is Immutable && this.Equals((Immutable)obj);
            }

            public override int GetHashCode()
            {
                return this.Value;
            }
        }

        public class WithProperty<T>
        {
            public WithProperty()
            {
            }

            public WithProperty(T value)
            {
                this.Value = value;
            }

            public T Value { get; set; }
        }

        public class WithIndexerType
        {
            public string Name { get; set; }

            // ReSharper disable once UnusedParameter.Global
            public int this[int index]
            {
                get { return 0; }
                set { }
            }
        }
    }
}
