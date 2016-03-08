namespace Gu.State
{
    using System;

    public interface IReference : IEquatable<IReference>
    {
    }

    //public struct Reference : IReference
    //{
    //    public bool Equals<T>(T other)
    //        where T : IReference
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (ReferenceEquals(null, obj))
    //        {
    //            return false;
    //        }
    //        return obj is Reference && Equals((Reference)obj);
    //    }

    //    public override int GetHashCode()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public static bool operator ==(Reference left, Reference right)
    //    {
    //        return left.Equals(right);
    //    }

    //    public static bool operator !=(Reference left, Reference right)
    //    {
    //        return !left.Equals(right);
    //    }
    //}
}