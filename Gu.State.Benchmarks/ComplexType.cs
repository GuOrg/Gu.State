// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Gu.State.Benchmarks
{
    public class ComplexType
    {
        public string Name { get; set; }

        public int Value { get; set; }

        protected bool Equals(ComplexType other)
        {
            return string.Equals(this.Name, other.Name) && this.Value == other.Value;
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
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return this.Equals((ComplexType)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Name?.GetHashCode() ?? 0) * 397) ^ this.Value;
            }
        }

        public static bool operator ==(ComplexType left, ComplexType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ComplexType left, ComplexType right)
        {
            return !Equals(left, right);
        }
    }
}