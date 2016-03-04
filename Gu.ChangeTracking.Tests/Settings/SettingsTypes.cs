namespace Gu.ChangeTracking.Tests.Settings
{
    using System;

    public static class SettingsTypes
    {
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
            // ReSharper disable once UnusedParameter.Global
            public int this[int index]
            {
                get { return 0; }
                set { }
            }
        }
    }
}
