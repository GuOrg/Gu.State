namespace Gu.ChangeTracking.Internals
{
    using System.Collections.Generic;
    using System.Reflection;

    internal class MemberInfoComparer<T> : IEqualityComparer<T>
        where T : MemberInfo
    {
        public static readonly MemberInfoComparer<T> Default = new MemberInfoComparer<T>();

        private MemberInfoComparer()
        {
        }

        public bool Equals(T x, T y)
        {
            return x.Name == y.Name && x.DeclaringType == y.DeclaringType;
        }

        /// <summary>
        /// http://stackoverflow.com/a/263416/1069200
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(T obj)
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = hash * 16777619 ^ obj.Name.GetHashCode();
                hash = hash * 16777619 ^ (obj.DeclaringType?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}
